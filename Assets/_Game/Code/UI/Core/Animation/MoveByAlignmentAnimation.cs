using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

namespace MH.UISystem.Animation
{
    public enum Alignment
    {
        None,
        Left,
        Top,
        Right,
        Bottom
    }

    [CreateAssetMenu(fileName = "MoveByAlignmentAnimation", menuName = "UISystem/Animation/Create Move By Alignment Animation", order = 0)]
    public class MoveByAlignmentAnimation : BaseViewAnimation
    {
        [SerializeField] private Alignment _from;
        [SerializeField] private Alignment _to;
        [SerializeField] private float     _startDelay;
        [SerializeField] private float     _duration = 0.25f;
        [SerializeField] private Ease      _ease     = Ease.Linear;

        public override async UniTask AnimateAsync(RectTransform rectTransform, CanvasGroup canvasGroup)
        {
            rectTransform.anchoredPosition = PositionFromAlignment(rectTransform, _from);
            /*await rectTransform.DOAnchorPos(PositionFromAlignment(rectTransform, _to), _duration)
                               .SetDelay(_startDelay).SetEase(_ease).SetUpdate(true).ToUniTask();*/
            
            bool completed = false;
            rectTransform.DOAnchorPos(PositionFromAlignment(rectTransform, _to), _duration)
                .SetDelay(_startDelay).SetEase(_ease).SetUpdate(true).OnComplete(()=> { completed = true; });
            
            await UniTask.WaitUntil(()=> completed);    
        }

        private Vector2 PositionFromAlignment(RectTransform rectTransform, Alignment alignment)
        {
            var rect = rectTransform.rect;
            return alignment switch
                   {
                       Alignment.Left   => Vector2.left  * rect.width,
                       Alignment.Top    => Vector2.up    * rect.height,
                       Alignment.Right  => Vector2.right * rect.width,
                       Alignment.Bottom => Vector2.down  * rect.height,
                       _                => Vector2.zero
                   };
        }
    }
}