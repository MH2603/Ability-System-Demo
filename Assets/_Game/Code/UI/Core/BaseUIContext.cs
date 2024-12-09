using System;
using UniRx;
using UnityEngine;

namespace MH.UISystem
{
    /// <summary>
    /// Base class for UI contexts.
    /// Override this class to create a new UI context with a specific model.
    /// </summary>
    public abstract class BaseUIContext<T> : UIContext where T: UIModel
    {
        public T Model { get; private set; }

        protected CompositeDisposable compositeDisposable;

        public virtual void Initialize(T model) 
        {
            compositeDisposable?.Dispose();
            compositeDisposable = new CompositeDisposable();
            compositeDisposable.AddTo(this);
            
            Model = model;
            OnShowing(model);
        }

        /// <summary>
        /// Override this method to handle the context initialization.
        /// </summary>
        /// <param name="model">The UI model associated with the context.</param>
        protected abstract void OnShowing(T model);

        private void OnDestroy()
        {
            compositeDisposable?.Dispose();
        }
    }
    
    /// <summary>
    /// Base class for UI contexts.
    /// Override this class to create a new UI context with a specific model.
    /// </summary>
    /// <typeparam name="TModel">The type of UI model.</typeparam>
    /// <typeparam name="TView">The type of UI view</typeparam>
    /// <typeparam name="TViewModel">The type of UI view model</typeparam>
    public abstract class BaseUIContext<TModel, TView, TViewModel> : BaseUIContext<TModel>
        where TModel : UIModel
        where TView : UIView<TViewModel>
        where TViewModel : UIViewModel
    {
        [field: SerializeField] public TView      View      { get; protected set; }
        [field: SerializeField] public TViewModel ViewModel { get; protected set; }
        
        public override void Initialize(TModel model)
        {
            ViewModel ??= Activator.CreateInstance<TViewModel>();
            View.SetViewModel(ViewModel);
            base.Initialize(model);
        }
    }
}