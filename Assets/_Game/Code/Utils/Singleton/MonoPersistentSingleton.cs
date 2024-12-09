using UnityEngine;

namespace Utils
{
    public class MonoPersistentSingleton<T> : MonoBehaviour where T : Component
    {
        public static bool HasInstance => _instance != null;
        public static T    Current     => _instance;

        protected static T    _instance;
        protected        bool _enabled;

        /// <summary>
        /// Singleton design pattern
        /// </summary>
        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<T>();
                    if (_instance == null)
                    {
                        GameObject obj = new GameObject ();
                        obj.name = typeof(T).Name + "_AutoCreated";
                        _instance = obj.AddComponent<T> ();
                    }
                }

                return _instance;
            }
        }

        /// <summary>
        /// On awake, we check if there's already a copy of the object in the scene. If there's one, we destroy it.
        /// </summary>
        protected virtual void Awake()
        {
            InitializeSingleton();
        }

        /// <summary>
        /// Initializes the singleton.
        /// </summary>
        protected virtual void InitializeSingleton()
        {
            if (!Application.isPlaying)
            {
                return;
            }

            if (_instance == null)
            {
                // Make this instance the singleton if it's the first instance
                _instance = this as T;
                DontDestroyOnLoad(transform.gameObject);
                _enabled = true;
            }
            else
            {
                //If a Singleton already exists and there is another GameObject, destroy it
                if (this != _instance)
                {
                    Destroy(this.gameObject);
                }
            }
        }
    }
}