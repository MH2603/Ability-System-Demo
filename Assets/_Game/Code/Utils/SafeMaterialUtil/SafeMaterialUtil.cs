using System;
using System.Collections.Generic;
using System.Diagnostics;
using JetBrains.Annotations;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Code.Utils.SafeMaterialUtil
{
    /*
This class is intended to fix the following issue: https://answers.unity.com/questions/548420/material-memory-leak.html
When accessing a Renderer's .material or .materials property, an instance of the material will be created. This instance
must be destroyed using the GameObject.Destroy() method.

Instead of calling .material or .materials directly on a Renderer, you should use the extension methods
.GetSafeMaterial() and .GetSafeMaterials(). Internally, a hash of instanced material will allow 
for tracking and destruction of material instances, when needed.
 */

// Note: SafeMaterialUtil implements IDisposable, so we can cleanup tracked materials when we fake-logout through the void scene. 
    public class SafeMaterialUtil : IDisposable
    {
        private class SafeMaterialEntry : IDisposable
        {
            public Material Material;
            public HashSet<GameObject> Owners;

            public void Dispose()
            {
#if UNITY_EDITOR
                Object.DestroyImmediate(Material);
#else
            Object.Destroy(Material);
#endif
                Material = null;
                Owners.Clear();
            }
        }

        private static SafeMaterialUtil Instance = new SafeMaterialUtil();

        public static Material GetMaterial([NotNull] Renderer renderer)
        {
            LogAssumeNotNull(Instance, "Got no SafeMaterialUtil Instance");
            LogAssumeNotNull(renderer, "Trying to GetMaterial() for a null Renderer");
            LogAssumeNotNull(renderer.gameObject, "Got null GameObject for renderer?");
            Material material = renderer.material;
            LogAssumeNotNull(material, "Got null Material for Renderer '{0}'", renderer.gameObject);
            Instance.Add(renderer.gameObject, material);
            return material;
        }

        public static Material[] GetMaterials([NotNull] Renderer renderer)
        {
            LogAssumeNotNull(renderer, "Trying to GetMaterial() for a null Renderer");
            Material[] materials = renderer.materials;
            for (int i = 0; i < materials.Length; i++)
            {
                Material material = materials[i];
                LogAssumeNotNull(material, "Got null Material for Renderer '{0}'", renderer.gameObject);
                Instance.Add(renderer.gameObject, material);
            }
            return materials;
        }

        public static Material TrackLooseMaterial([NotNull] GameObject owner, [NotNull] Material material)
        {
            LogAssumeNotNull(owner, "Got null GameObject for as sponsor for a tracked Material");
            LogAssumeNotNull(material, "Got null Material for owner '{0}'", owner);
            Instance.Add(owner, material);
            return material;
        }

        public static void CleanupOrphanedMaterials() => Instance.DestroyOrphanedMaterials();

        private bool IsAlreadyDisposed;

        private SafeMaterialUtil() {}

        public void Dispose()
        {
            if (IsAlreadyDisposed)
            {
                return;
            }

            foreach (SafeMaterialEntry materialEntry in TrackedMaterials.Values)
            {
                materialEntry.Dispose();
            }
            TrackedMaterials.Clear();

            GC.SuppressFinalize(this);

            Instance = null;
            IsAlreadyDisposed = true;
        }

        // For each tracked Material Instance we also track all the GameObjects that own this particular instance.
        // If we encounter a Matarial without owners we destroy it.
        // Using Material as Dictionary key would lead to many material.GetInstanceID() calls for each Dictionary operation,
        // so we use int material.GetInstanceID() as key directly instead.
        private readonly Dictionary<int, SafeMaterialEntry> TrackedMaterials = new Dictionary<int, SafeMaterialEntry>(2048);
        private readonly List<int> DestroyedMaterialIDs = new List<int>();
        private void DestroyOrphanedMaterials()
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            foreach (KeyValuePair<int, SafeMaterialEntry> kvp in TrackedMaterials)
            {
                int matID = kvp.Key;
                SafeMaterialEntry materialEntry = kvp.Value;

                // Clear out all the owning GameObjects that have died since we last called this method RIP.
                materialEntry.Owners.RemoveWhere(g => g == null);

                // If no one is left, we can kill this material. Double RIP
                if (materialEntry.Owners.Count == 0)
                {
                    DestroyedMaterialIDs.Add(matID);
                    materialEntry.Dispose();
                }
            }

            // Finally, remove the entries we dont need anymore (because they are destroyed) from the dict
            for (int i = 0; i < DestroyedMaterialIDs.Count; i++)
            {
                TrackedMaterials.Remove(DestroyedMaterialIDs[i]);
            }

            UnityEngine.Debug.LogFormat("[SafeMaterialUtil] Cleaning up {0} TrackedMaterials in {1} ms", DestroyedMaterialIDs.Count, stopwatch.ElapsedMilliseconds);
            DestroyedMaterialIDs.Clear();
        }

#if UNITY_EDITOR
        // We track materials in editor, both in play and edit more. Dleanup orphans when we change playmode
        // so the UnityEditor doesn't gobble up memory.
        public static void OnPlayModeChanged(UnityEditor.PlayModeStateChange change)
        {
            Instance.DestroyOrphanedMaterials();
        }
#endif

        private void Add(GameObject owner, [NotNull] Material materialInstance)
        {
            int matId = materialInstance.GetInstanceID();

            if (!TrackedMaterials.TryGetValue(matId, out SafeMaterialEntry materialEntry))
            {
                materialEntry = new SafeMaterialEntry
                {
                    Material = materialInstance,
                    Owners = new HashSet<GameObject>()
                };
                TrackedMaterials.Add(matId, materialEntry);
            }

            materialEntry.Owners.Add(owner);
        }

        private static void LogAssumeNotNull(object x, string optionalMessageFormat = null, object optionalParam = null)
        { 
            if (x != null) {
                // UnityEngine.Object defines operator==, operator!= and the operator bool() to check whether the
                // underlying native object has been destroyed. However, the operator is determined at compile time, so when we pass in
                // a destroyed UnityEngine.Object (x != null) will evaluate to true, and we return when we should not.
                // This additional check fixes this.
                if ((x is UnityEngine.Object) ? (UnityEngine.Object) x : true)
                {
                    return;
                }
            }

            string logString = "SafeMaterialUtil: Assumed wrong ,";
            if (optionalMessageFormat != null)
            {
                logString += string.Format(optionalMessageFormat, optionalParam);
            }
            UnityEngine.Debug.Log(logString);
#if DEV_BUILD
            throw new Exception(logString);
#endif
        }
    }


// Extension methods on Material
    public static class SafeMaterials
    {
        public static Material GetSafeMaterial(this Renderer renderer) => SafeMaterialUtil.GetMaterial(renderer);

        public static Material[] GetSafeMaterials(this Renderer renderer) => SafeMaterialUtil.GetMaterials(renderer);
    }
}
