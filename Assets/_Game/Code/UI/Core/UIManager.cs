using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Utils;

namespace MH.UISystem
{
    public sealed class UIManager : MonoSingleton<UIManager>
    {
        [SerializeField] private List<UILayer> _layers = new List<UILayer>();

        private readonly Dictionary<Type, UILayer> _cachedLayers = new Dictionary<Type, UILayer>();

        protected override void Awake()
        {
            CacheUILayers();
        }


        /// <summary>
        /// Retrieve the main instance of a specific UILayer.
        /// </summary>
        /// <typeparam name="T">The specific UILayer type.</typeparam>
        /// <returns>The main instance of the specified UILayer type.</returns>
        public T GetMain<T>() where T : UILayer<T>
        {
            var requestedType = typeof(T);
            if (_cachedLayers.TryGetValue(requestedType, out UILayer layer))
            {
                return layer as T;
            }
        
            Debug.LogError($"UI layer of type {requestedType} not found.");
            return null;
        }

        /// <summary>
        /// Register a UILayer instance in the UIManager.
        /// </summary>
        /// <typeparam name="T">The specific UILayer type.</typeparam>
        /// <param name="layer">The instance of the UILayer to be registered.</param>
        public void RegisterLayer<T>(T layer) where T : UILayer<T>
        {
            var layerType = typeof(T);
            if (_cachedLayers.TryAdd(layerType, layer))
            {
                _layers.Add(layer);
            }
            else
            {
                Debug.LogError($"UI layer of type {layerType} already registered.");
            }
        }


        /// <summary>
        /// Sets the active state of a specific UILayer asynchronously.
        /// </summary>
        /// <typeparam name="T">The specific UILayer type.</typeparam>
        /// <param name="active">True to activate the layer, false to deactivate it.</param>
        /// <returns>A UniTask representing the asynchronous operation.</returns>
        public async UniTask SetActiveLayerAsync<T>(bool active) where T : UILayer<T>
        {
            var layer = GetMain<T>();
            if (!layer)
            {
                Debug.LogError($"Layer {typeof(T).Name} not found.");
            }
            await layer.SetActiveAsync(active);
        }


        /// <summary>
        /// Cache the instances of UILayers in the UIManager.
        /// </summary>
        private void CacheUILayers()
        {
            foreach (var layer in _layers)
            {
                var layerType = layer.GetType();
                if (!_cachedLayers.TryAdd(layerType, layer))
                {
                    Debug.LogError($"Duplicate UI layer of type {layerType} found. Only the first instance will be used.");
                }
            }
        }
    }
}