using Cysharp.Threading.Tasks;
using UnityEngine;

namespace MH.UISystem.Animation
{
    public abstract class BaseViewAnimation : ScriptableObject
    {
        public abstract UniTask AnimateAsync(RectTransform rectTransform, CanvasGroup canvasGroup);
    }
}