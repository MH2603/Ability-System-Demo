using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Serialization;

// ReSharper disable Unity.NoNullPropagation

namespace MH.UISystem.Animation
{
    [CreateAssetMenu(fileName = "ViewAnimation", menuName = "UISystem/Add View Animation", order = 0)]
    public class ViewAnimation : ScriptableObject
    {
        #region Fields

        [SerializeReference] public BaseViewAnimation[] _viewAnimations;

        #endregion

        #region Public Methods
        
        public async UniTask AnimateAsync(Transform transform, CanvasGroup canvasGroup) => await AnimateAsync((RectTransform)transform, canvasGroup);

        /// <summary>
        /// Animates the given Rect transform and canvas group using the move animations specified in the <see cref="ViewAnimation"/> scriptable object.
        /// </summary>
        /// <param name="rectTransform">The Rect transform to animate.</param>
        /// <param name="canvasGroup">The canvas group to animate.</param>
        /// <returns>A <see cref="UniTask"/> representing the asynchronous operation.</returns>
        public async UniTask AnimateAsync(RectTransform rectTransform, CanvasGroup canvasGroup)
        {
            var tasks = new UniTask[_viewAnimations.Length];
            for (var i = 0; i < _viewAnimations.Length; i++)
            {
                tasks[i] = _viewAnimations[i].AnimateAsync(rectTransform, canvasGroup);
            }
            await UniTask.WhenAll(tasks);
        }

        #endregion
    }
    
}