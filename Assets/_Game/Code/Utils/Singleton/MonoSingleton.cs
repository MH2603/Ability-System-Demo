using UnityEngine;

namespace Utils
{
    /// <summary>
    /// Base class for creating singleton MonoBehaviour classes.
    /// </summary>
    /// <typeparam name="T">The type of the derived class.</typeparam>
    public class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
    {
        private static T _instance;

        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<T>();
                    if (_instance == null)
                    {
                        Debug.LogError("An instance of " + typeof(T) + " is needed in the scene, but there is none.");
                    }
                }

                return _instance;
            }
        }

        protected virtual void Awake()
        {
            if (_instance == null)
            {
                _instance = (T) this;
            }
            else if (_instance != this)
            {
                Debug.LogError("An instance of " + typeof(T) + " already exists in the scene.");
                Destroy(gameObject);
            }
        }

        protected virtual void OnDestroy()
        {
            if (_instance == this)
            {
                _instance = null;
            }
        }
    }
}