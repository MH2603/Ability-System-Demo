using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MH.UISystem
{
    // <summary>
    /// Base class for reactive UI views.
    /// Implement this class to create a reactive view that can bind to view model you want.
    /// </summary>
    /// <typeparam name="T">Type of the view model.</typeparam>
    public abstract class UIView<T> : UIBehaviour where T : UIViewModel
    {
        protected                         CompositeDisposable compositeDisposable;
        [field: SerializeField] protected T                   viewModel;

        public T ViewModel => viewModel;

        /*[MMInspectorButton("ManualSetViewModel")]*/ [SerializeField]
        private bool _manualSetViewModel;

        /// <summary>
        /// Sets the view model for the UIView.
        /// </summary>
        /// <param name="viewModel">The view model to set.</param>
        public void SetViewModel(T viewModel)
        {
            compositeDisposable?.Dispose();

            compositeDisposable = new CompositeDisposable();
            compositeDisposable.AddTo(this);
            
            this.viewModel = viewModel;
            OnSetViewModel(viewModel);
        }

        /// <summary>
        /// This method is only used for the inspector button.
        /// DO NOT USE THIS METHOD IN CODE.
        /// </summary>
        public void ManualSetViewModel()
        {
            OnSetViewModel(viewModel);
        }
        
        protected abstract void OnSetViewModel(T vModel);
    }
}