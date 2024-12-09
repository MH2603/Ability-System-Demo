using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace MH.UISystem.HUD
{
    public class HUDBaseLayer : UIBaseLayer<HUDBaseLayer>
    {
        /// <summary>
        /// Shows the specified HUD instance asynchronously.
        /// </summary>
        /// <typeparam name="TContext">The type of the HUD instance.</typeparam>
        /// <typeparam name="TViewModel">The type of the view model.</typeparam>
        /// <param name="viewModel">The view model instance (optional).</param>
        /// <param name="onPreInitialize">The action to be invoked before initialization (optional).</param>
        /// <param name="onPostInitialize">The action to be invoked after initialization (optional).</param>
        /// <returns>A UniTask representing the asynchronous operation.</returns>
        /// <exception cref="ArgumentNullException">Thrown if the HUD instance can't be found.</exception>
        public async UniTask<TContext> ShowAsync<TContext, TViewModel>(
            TViewModel viewModel        = null,
            Action<TContext>  onPreInitialize  = null,
            Action<TContext>  onPostInitialize = null)
            where TContext : BaseUIContext<TViewModel>
            where TViewModel : UIModel
        {
            if (!TryGetContextInstance<TContext>(out var hudInstance))
            {
                Debug.LogError($"HUD not found: {typeof(TContext)}");
                return null;
            }

            return await ShowAsyncInternal(hudInstance, viewModel, onPreInitialize, onPostInitialize);
        }

        /// <summary>
        /// Hides the specified UI context instance asynchronously.
        /// </summary>
        /// <typeparam name="TContext">The type of the UI context instance.</typeparam>
        /// <returns>A UniTask representing the asynchronous operation.</returns>
        /// <exception cref="ArgumentNullException">Thrown if the UI context instance can't be found.</exception>
        public async UniTask<TContext> HideAsync<TContext>() where TContext : UIContext
        {
            if (_contextRuntimeMap.TryGetValue(typeof(TContext), out var hudInstance))
            {
                await hudInstance.HideAsync();
                return hudInstance as TContext;
            }

            Debug.LogError($"HUD not found: {typeof(TContext)}");
            return null;
        }
    }
}