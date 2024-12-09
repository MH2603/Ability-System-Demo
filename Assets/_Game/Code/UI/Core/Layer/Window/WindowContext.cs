using UnityEngine;

namespace MH.UISystem.Window
{
    public interface IWindowContext
    {
        bool StackToWindowLayer { get; }
    }


    /// <summary>
    /// Represents a context for a window in a user interface.
    /// </summary>
    /// <typeparam name="TModel">The model type associated with the window.</typeparam>
    /// <typeparam name="TView">The view type associated with the window.</typeparam>
    /// <typeparam name="TViewModel">The view model type associated with the window.</typeparam>
    public abstract class WindowContext<TModel, TView, TViewModel> : BaseUIContext<TModel, TView, TViewModel>,
        IWindowContext
        where TModel : UIModel
        where TView : UIView<TViewModel>
        where TViewModel : UIViewModel
    {
        /// <summary>
        /// Gets a value indicating whether the window should be stacked on top of the window layer.
        /// When a window is stacked, the previous window will be displayed when the current window is closed.
        /// </summary>
        /// <remarks>
        /// If this property is set to true, the window will be added to the window stack when shown.
        /// If this property is set to false, the window will be displayed on top of the window layer and replace the current window.
        /// The window stack is managed by the <see cref="WindowBaseLayer"/> class.
        /// When a window is closed, the previous window in the stack will be displayed.
        /// </remarks>
        /// <seealso cref="WindowBaseLayer"/>
        /// <seealso cref="WindowBaseLayer.ShowAsyncInternal()"/>
        /// <seealso cref="IWindowContext"/>
        [Tooltip("If true, the window will be stacked on top of the window layer. When closing the window, the previous window will be displayed.")]
        [field: SerializeField] public bool       StackToWindowLayer { get; protected set; }
        
    }
}