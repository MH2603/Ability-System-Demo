using System;
using Cysharp.Threading.Tasks;
using MH.UISystem.Animation;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

namespace MH.UISystem
{

    public enum EVisibleState
    {
        Appearing,
        Appeared,
        Disappearing,
        Disappeared,
    }

    /// <summary>
    /// Represents the base class for UI Models.
    /// </summary>
    public abstract class UIModel
    {
        
    }

    
    public abstract class UIViewModel
    {
        
    }
    
    /// <summary>
    /// Represent the base class for UI contexts
    /// </summary>
    /// <remarks>
    /// This class provides functionality for UI views. It manages the visibility and lifecycle of the UI view.
    /// </remarks>
    [RequireComponent(typeof(CanvasGroup))]
    [RequireComponent(typeof(Canvas))]
    [RequireComponent(typeof(GraphicRaycaster))]
    public abstract class UIContext : MonoBehaviour
    {
        #region  ----------- Properties ----------------

        private CanvasGroup _canvasGroup;
        [field: SerializeField] public bool SpawnOnAwake { get; private set; }
        [SerializeField] private ViewAnimation _showAnimation;
        [SerializeField] private ViewAnimation _hideAnimation;

        public float LastShowTime { get; private set; }
        public UILayer UILayer { get;  set; }
        
        public CanvasGroup   CanvasGroup   => _canvasGroup ? _canvasGroup : _canvasGroup = GetComponent<CanvasGroup>();
        public EVisibleState EVisibleState { get; private set; } = EVisibleState.Disappeared;

        #endregion

        #region  ----- Observables ------
        
        private Subject<UIContext> _preAppear     = new();
        private Subject<UIContext> _postAppear    = new();
        private Subject<UIContext> _preDisappear  = new();
        private Subject<UIContext> _postDisappear = new();
        public IObservable<UIContext> OnPreAppear     => _preAppear.Share();
        public IObservable<UIContext> OnPostAppear    => _postAppear.Share();
        public IObservable<UIContext> OnPreDisappear  => _preDisappear.Share();
        public IObservable<UIContext> OnPostDisappear => _postDisappear.Share();
        
        #endregion


        #region  ----------- Public Methods ------------------

        /// <summary>
        /// This logic is executed when the UI View is activated.
        /// Activate game object -> Change to Appearing State -> Send AppearEvent ->
        /// -> Wait for pre-processing logic -> Proceed with Show animation -> Change to Appeared State -> Send AppearedEvent 
        /// </summary>
        public async UniTask ShowAsync(bool useAnimation = true)
        {
            if (EVisibleState is EVisibleState.Appearing or EVisibleState.Appeared)
            {
                return;
            }

            LastShowTime = Time.time;
            
            EVisibleState = EVisibleState.Appearing;
            _preAppear.OnNext(this);
            
            var rectTransform = (RectTransform)transform;   
            await InitializeRectTransformAsync(rectTransform);
            
            CanvasGroup.alpha = 1;  
            gameObject.SetActive(true);

            if (useAnimation && _showAnimation != null)
            {
                await _showAnimation.AnimateAsync(rectTransform, CanvasGroup);
            }
            
            EVisibleState = EVisibleState.Appeared;
            _postAppear.OnNext(this);
        }
        
        /// <summary>
        /// This logic is executed when the UI View is activated.
        /// Change to Disappearing State -> Send DisappearEvent -> Proceed with Hide animation -> Wait for post-processing logic -> Change to Disappeared State -> Send DisappearedEvent
        /// </summary>
        internal async UniTask HideAsync(bool useAnimation = true)
        {
            // We cannot hide the view if it is already disappeared or disappearing.
            if (EVisibleState is EVisibleState.Disappeared or EVisibleState.Disappearing)
                return;

            EVisibleState = EVisibleState.Disappearing;
            _postDisappear.OnNext(this);

            // auto return if gameobject is destroy
            await UniTask.Yield(cancellationToken: this.GetCancellationTokenOnDestroy());

            if (useAnimation && _hideAnimation != null)
                await _hideAnimation.AnimateAsync(transform, CanvasGroup);

            gameObject.SetActive(false);

            EVisibleState = EVisibleState.Disappeared;
            _postAppear.OnNext(this);
        }

        #endregion

        #region  ---------- Private Methods ---------------

        /// <summary>
        /// Reset RectTransform to default values.
        /// </summary>
        /// <param name="rectTransform"></param>
        private async UniTask InitializeRectTransformAsync(RectTransform rectTransform)
        {
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.sizeDelta = Vector2.zero;
            await UniTask.Yield();
            rectTransform.anchoredPosition = Vector2.zero;
            rectTransform.localScale       = Vector3.one;
            rectTransform.localRotation    = Quaternion.identity;
        }

        #endregion
        
        
    }

}
