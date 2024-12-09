using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace MH.UISystem.Animation
{
    [CreateAssetMenu(fileName = "FadeAnimation", menuName = "UISystem/Animation/Create Fade Animation", order = 0)]
    public class FadeAnimation : BaseViewAnimation
    {
        [SerializeField] private float _from;
        [SerializeField] private float _to;
        [SerializeField] private float _startDelay;
        [SerializeField] private float _duration = 0.25f;
        [SerializeField] private Ease  _ease     = Ease.Linear;

        public override async UniTask AnimateAsync(RectTransform rectTransform, CanvasGroup canvasGroup)
        {
            canvasGroup.alpha = _from;
            bool completed = false;
            /*await canvasGroup.DOFade(_to, _duration).SetDelay(_startDelay).SetEase(_ease).SetUpdate(true).ToUniTask();*/
            canvasGroup.DOFade(_to, _duration).SetDelay(_startDelay).SetEase(_ease).SetUpdate(true)
                .OnComplete(() => completed = true);
            
            await UniTask.WaitUntil( () =>completed );
        }
    }
}