using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Serialization;
using UniRx;

namespace MH.UISystem
{
    /// <summary>
    /// The base class for all UI Layers
    /// </summary>
    public abstract class UIBaseLayer<T> : UILayer<T> where T : UIBaseLayer<T>  
    {
        #region  ---------- Inspectors ---------------

        [FormerlySerializedAs("_contexts")] [SerializeField] protected UIContext[] _contextPrefabs;

        #endregion


        #region  ------------- Properties -----------------

        protected readonly Dictionary<Type, UIContext> _contextRuntimeMap = new();
        protected readonly Dictionary<Type, UIContext> _contextPrefabMap = new();

        public UIContext CurrentView { get; protected set; }

        #endregion

        #region  -------------- Unity Methods ---------

        private void Awake()
        {
            InitializeContexts(_contextPrefabs);
        }

        #endregion


        #region  --------------- Protected Methods ---------------

        protected void InitializeContexts(UIContext[] contexts)
        {
            foreach (var contextPrefab in _contextPrefabs)
            {
                _contextPrefabMap.Add(contextPrefab.GetType(), contextPrefab);
                if (contextPrefab.SpawnOnAwake)
                {
                    GetOrCreateContextInstance(contextPrefab);    
                }
            }
        }

        /// <summary>
        /// Gets or create an instance of the specificed UI context type
        /// </summary>
        /// <param name="contextPrefab"> The Ui Context to get or create an instance of</param>
        /// <typeparam name="TContext"> The type of the UI context </typeparam>
        /// <returns>The instance of the UI context.</returns>
        protected TContext GetOrCreateContextInstance<TContext>(TContext contextPrefab) where TContext : UIContext 
        {
            if (!_contextRuntimeMap.TryGetValue(typeof(TContext), out var contextInstance))
            {
                contextPrefab.gameObject.SetActive(false);
                contextInstance = Instantiate(contextPrefab, transform);
                contextPrefab.gameObject.SetActive(true);
                _contextRuntimeMap.Add(typeof(TContext), contextInstance);  
                
            }
            
            return contextInstance as TContext;
        }

        
        /// <summary>
        /// Tries to get an instance of the specified UI context type.
        /// </summary>
        /// <typeparam name="TContext">The type of the UI context.</typeparam>
        /// <param name="contextInstance">The instance of the UI context.</param>
        /// <returns><c>true</c> if the instance is found or created successfully; otherwise, <c>false</c>.</returns>
        protected bool TryGetContextInstance<TContext>(out TContext contextInstance) where TContext : UIContext
        {
            contextInstance = null;
            if ( !_contextPrefabMap.TryGetValue(typeof(TContext), out var context) )
            {
                return false;
            }

            if (!_contextRuntimeMap.TryGetValue(typeof(TContext), out var instance))
            {
                instance = GetOrCreateContextInstance(context); 
            }
            
            contextInstance = instance as TContext;
            return true;
        }

        /// <summary>
        /// Shows the specified UI context asynchronously.
        /// </summary>
        /// <typeparam name="TContext">The type of the UI context.</typeparam>
        /// <typeparam name="TModel">The type of the UI view model.</typeparam>
        /// <param name="contextInstance">The instance of the UI context.</param>
        /// <param name="viewModel">The view model to initialize the UI context.</param>
        /// <param name="onPreInitialize">The action to perform before initializing the UI context.</param>
        /// <param name="onPostInitialize">The action to perform after initializing the UI context.</param>
        /// <returns>An instance of TContext if the instance is found or created successfully; otherwise, null.</returns>
        protected async UniTask<TContext> ShowAsyncInternal<TContext, TModel>(
                    TContext contextInstance,
                    TModel viewModel,
                    Action<TContext> onPreInitialize,
                    Action<TContext> onPostInitialize) 
                    where TContext : BaseUIContext<TModel>
                    where TModel : UIModel
        {
            if (IsCurrentViewBusy())
            {
                return null;
            }
            
            SetupContextInstance(contextInstance, viewModel, onPreInitialize, onPostInitialize);

            await CurrentView.ShowAsync();
            
            return CurrentView as  TContext;    
        }
        
        protected bool IsCurrentViewBusy()
        {
            return CurrentView != null &&
                   (CurrentView.EVisibleState == EVisibleState.Appearing ||
                    CurrentView.EVisibleState == EVisibleState.Disappearing);
        }


        /// <summary>
        /// Sets up an instance of a UI context with the provided parameters.
        /// </summary>
        /// <typeparam name="TContext">The type of the UI context.</typeparam>
        /// <typeparam name="TViewModel">The type of the UI view model.</typeparam>
        /// <param name="contextInstance">The instance of the UI context to be setup.</param>
        /// <param name="viewModel">The view model to be assigned to the UI context (can be null).</param>
        /// <param name="onPreInitialize">An optional callback to be invoked before the UI context appears.</param>
        /// <param name="onPostInitialize">An optional callback to be invoked after the UI context appears.</param>
        protected void SetupContextInstance<TContext, TViewModel>(
            TContext contextInstance,
            TViewModel viewModel,
            Action<TContext> onPreInitialize,
            Action<TContext> onPostInitialize)
            where TContext : BaseUIContext<TViewModel>
            where TViewModel : UIModel
        {
            
            // Subscribe to the OnPreAppear observable of contextInstance.
            // This subscription triggers the onPreInitialize action with contextInstance as a parameter
            // whenever the OnPreAppear observable emits an event.
            contextInstance.OnPreAppear
                .Subscribe(_ => onPreInitialize?.Invoke(contextInstance))
                // Use AddTo to bind the subscription's lifecycle to contextInstance.
                // When contextInstance is destroyed or disposed, the subscription will be automatically disposed.
                .AddTo(contextInstance);
            
            if (viewModel != null)
            {
                contextInstance.Initialize(viewModel);
            }

            contextInstance.OnPostAppear.Subscribe(_ => onPostInitialize?.Invoke(contextInstance)).AddTo(contextInstance);

            contextInstance.UILayer = this;
            CurrentView = contextInstance;
        }

        #endregion
    }
}