using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace MH.UISystem.Popup
{
    /// <summary>
    /// Base class for popup layers.
    /// </summary>
    /// <typeparam name="T">The derived type of the popup layer.</typeparam>
    public class PopupBaseLayer : UIBaseLayer<PopupBaseLayer>
    {
        private readonly Stack<UIContext> _popupStack = new();

        /// <summary>
        /// Displays a popup asynchronously.
        /// </summary>
        /// <typeparam name="T">The derived type of the popup layer.</typeparam>
        /// <typeparam name="TViewModel">The type of the view model.</typeparam>
        /// <param name="viewModel">Optional view model for the popup.</param>
        /// <param name="onPreInitialize">An action to be executed before initializing the popup.</param>
        /// <param name="onPostInitialize">An action to be executed after initializing the popup.</param>
        /// <returns>A UniTask representing the asynchronous operation that returns the popup instance.</returns>
        public async UniTask<T> ShowAsync<T, TViewModel>(
            TViewModel viewModel        = null,
            Action<T>  onPreInitialize  = null,
            Action<T>  onPostInitialize = null)
            where T : BaseUIContext<TViewModel>, IPopupContext
            where TViewModel : UIModel
        {
            if (!TryGetContextInstance<T>(out var popupInstance))
            {
                Debug.LogError($"Popup not found: {typeof(T)}");
                return null;
            }

            var result = await ShowAsyncInternal(popupInstance, viewModel, onPreInitialize, onPostInitialize);
            _popupStack.Push(result);
            return result;
        }

        /// <summary>
        /// Navigates back in the popup layer asynchronously.
        /// </summary>
        /// <param name="count">The number of popups to go back. Default is 1.</param>
        /// <param name="clearStack">Indicates whether to clear the entire popup stack. Default is false.</param>
        /// <returns>A UniTask representing the asynchronous operation.</returns>
        public async UniTask BackAsync(int count = 1, bool clearStack = false)
        {
            if (clearStack)
            {
                await ClearPopupStackAsync();
                return;
            }

            await HidePopupsAsync(count);
        }

        private async UniTask ClearPopupStackAsync()
        {
            while (_popupStack.TryPop(out var popup))
            {
                await popup.HideAsync();
            }
        }

        private async UniTask HidePopupsAsync(int count)
        {
            for (int i = 0; i < count; i++)
            {
                if (_popupStack.TryPop(out var popup))
                {
                    await popup.HideAsync();
                }
                else
                {
                    break;
                }
            }
        }
    }
}