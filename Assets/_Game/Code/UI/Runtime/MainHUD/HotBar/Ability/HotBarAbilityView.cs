using MH.UISystem;
using TMPro;
using UniRx;
using UnityEngine;  
using UnityEngine.UI;  

namespace MH.UI.Ability
{
    public class HotBarAbilityModel
    {
        public ReactiveProperty<string> AbilityName { get; set; } = new();
        public ReactiveProperty<Sprite> AbilityAvatar = new();
        public ReactiveProperty<KeyCode> KeyCode  = new();
        
        public ReactiveProperty<bool> IsCoolingDown = new();
        public ReactiveProperty<float> RemainCoolDown = new();
        public ReactiveProperty<float> CoolDownDuration = new();

        public ReactiveProperty<bool> IsCasting = new();
        
        public ReactiveProperty<bool> IsManaEnough = new();
    }
    
    public class HotBarAbilityViewModel : UIViewModel
    {
        public ReactiveProperty<string> AbilityName { get; set; } = new();
        public ReactiveProperty<Sprite> AbilityIcon { get; set; } = new();
        public ReactiveProperty<KeyCode> KeyCode  = new();
        
        public ReactiveProperty<bool> IsCoolingDown = new();
        public ReactiveProperty<float> RemainCoolDown = new();
        public ReactiveProperty<float> CoolDownDuration = new();

        public ReactiveProperty<bool> IsCasting = new();
        
        public ReactiveProperty<bool> IsManaEnough = new();
        
        public Subject<string> OnAbilityClicked = new();
        
    }
        
    
    public class HotBarAbilityView : UIView<HotBarAbilityViewModel>
    {
        #region  ---------- Inspectors -------------

        [SerializeField] private Image abilityIcon;
        [SerializeField] private TextMeshProUGUI castKetCodeText;
        [SerializeField] private Button useAbilityButton;
        
        [Header(" ---- Cool Down ----")]
        [SerializeField] private GameObject coolDownVisual;
        [SerializeField] private TextMeshProUGUI remainCoolDownText;
        [SerializeField] private Image remainCoolDownFillImage;
        
        [Header(" ---- Mana ----")]
        [SerializeField] private GameObject notEnoughManaVisual;

        [SerializeField] private GameObject castingViual;
        
        #endregion
        
        protected override void OnSetViewModel(HotBarAbilityViewModel vModel)
        {
            vModel.AbilityIcon.Subscribe( sprite => abilityIcon.sprite = sprite ).AddTo(compositeDisposable);
            vModel.KeyCode.Subscribe( keyCode => castKetCodeText.text = keyCode.ToString()).AddTo(compositeDisposable);
            
            vModel.IsCoolingDown.Subscribe(value => UpdateCoolDownVisual()).AddTo(compositeDisposable);
            vModel.RemainCoolDown.Subscribe(value => UpdateCoolDownVisual()).AddTo(compositeDisposable);
            vModel.CoolDownDuration.Subscribe(value => UpdateCoolDownVisual()).AddTo(compositeDisposable);
            
            vModel.IsCasting.Subscribe( ToggleCastingVisual ).AddTo(compositeDisposable);
            
            vModel.IsManaEnough.Subscribe(ToggleManaEnoughVisual).AddTo(compositeDisposable);  
            
            useAbilityButton.OnClickAsObservable().
                            Subscribe( _=> vModel.OnAbilityClicked.OnNext(vModel.AbilityName.Value) ).
                            AddTo(compositeDisposable);
        }

        private void UpdateCoolDownVisual()
        {
            coolDownVisual.SetActive(ViewModel.IsCoolingDown.Value);
            if(!ViewModel.IsCoolingDown.Value) return;
            
            remainCoolDownText.text = $"{ViewModel.RemainCoolDown.Value:F1}";
            remainCoolDownFillImage.fillAmount = ViewModel.RemainCoolDown.Value / ViewModel.CoolDownDuration.Value;
            // Debug.Log(" LOg Update Cool Down Fill !");
        }

        private void ToggleCastingVisual(bool on)
        {
            castingViual.SetActive(on); 
        }

        private void ToggleManaEnoughVisual(bool on)
        {
            notEnoughManaVisual.SetActive(on);
        }
    }
}