using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace MH.UISystem.Window
{
    /// <summary>
    /// Represents a base layer for windows within the UI system.
    /// </summary>
    /// <typeparam name="T">The type of the derived window context.</typeparam>
    public class WindowBaseLayer : UIBaseLayer<WindowBaseLayer>
    {
        // stack like most list but Stack is last it -> first out
        private readonly Stack<UIContext> _windowStack = new();

        /// <summary>
        /// Asynchronously shows a window of type <typeparamref name="T"/> with the specified view model and optional pre and post-initialization actions.
        /// </summary>
        /// <typeparam name="T">The type of the window context.</typeparam>
        /// <typeparam name="TViewModel">The type of the view model.</typeparam>
        /// <param name="viewModel">The view model to be passed to the window context.</param>
        /// <param name="onPreInitialize">Optional action to be executed before the window initialization.</param>
        /// <param name="onPostInitialize">Optional action to be executed after the window initialization.</param>
        /// <returns>A <see cref="UniTask"/> representing the asynchronous operation. The result is the window context instance.</returns>
        public async UniTask<T> ShowAsync<T, TViewModel>(
            TViewModel viewModel        = null,
            Action<T>  onPreInitialize  = null,
            Action<T>  onPostInitialize = null)
            where T : BaseUIContext<TViewModel>, IWindowContext
            where TViewModel : UIModel
        {
            if (!TryGetContextInstance<T>(out var windowInstance))
            {
                Debug.LogError($"Window not found: {typeof(T)}");
                return null;
            }

            var previousView = CurrentView;
            var result       = await ShowAsyncInternal(windowInstance, viewModel, onPreInitialize, onPostInitialize);

            if (previousView != null && previousView.EVisibleState == EVisibleState.Appeared)
            {
                // A ?? B: if A is null -> return B and If A not null -> return B
                await HandlePreviousView(previousView, result?.StackToWindowLayer ?? false);
            }

            return result;
        }

        /// <summary>
        /// Asynchronously goes back to the previous window in the window stack.
        /// </summary>
        /// <param name="clearStack">If set to <c>true</c>, clear the entire window stack.</param>
        public async UniTask BackAsync(bool clearStack = false)
        {
            if (clearStack)
            {
                _windowStack.Clear();
            }

            if (CurrentView == null)
            {
                Debug.LogError("No window to go back to.");
                return;
            }

            await CurrentView.HideAsync();

            if (_windowStack.TryPop(out var previousView))
            {
                await previousView.ShowAsync();
                CurrentView = previousView;
            }
            else
            {
                CurrentView = null;
            }
        }

        private async UniTask HandlePreviousView(UIContext previousView, bool stackToWindowLayer)
        {
            if (stackToWindowLayer)
            {
                _windowStack.Push(previousView);
            }

            await previousView.HideAsync();
        }
    }
}