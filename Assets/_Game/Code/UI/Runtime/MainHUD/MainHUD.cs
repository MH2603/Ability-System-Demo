using System.Collections.Generic;
using System.Linq;
using MH.UI.Ability;
using MH.UI.Ability.RuntimeStats;
using MH.UISystem;
using MH.UISystem.HUD;
using UniRx;

namespace MH.UI
{
    public class MainHUDModel : UIModel
    {
        public ReactiveProperty<float> MaxHp { get; set; } = new();
        public ReactiveProperty<float> CurrentHp { get; set; } = new();
        public ReactiveProperty<float> MaxMana { get; set; } = new();
        public ReactiveProperty<float> CurrentMana { get; set; } = new();
        
        public Dictionary<string ,HotBarAbilityViewModel> AbilityVModelMap = new(); 
        public Dictionary<string, RuntimeStatViewModel> StatVModelMap = new();
    }
    
    public class MainHUD : HUDContext<MainHUDModel, MainHUDView, MainHUDViewModel>
    {

        public override void Initialize(MainHUDModel model)
        {
            ViewModel = new();
            ViewModel.HotBarViewModel.Abilities = model.AbilityVModelMap.Values.ToList();
            ViewModel.HotBarViewModel.Stats = model.StatVModelMap.Values.ToList();
            base.Initialize(model); 
            
        }
        
        protected override void OnShowing(MainHUDModel model)
        {
            model.MaxHp.Subscribe(maxValue => ViewModel.CharacterStatBarViewModel.HpBarViewModel.MaxValue.Value = maxValue).AddTo(compositeDisposable);
            model.CurrentHp.Subscribe(value => ViewModel.CharacterStatBarViewModel.HpBarViewModel.CurrentValue.Value = value).AddTo(compositeDisposable);  
            model.MaxMana.Subscribe(value => ViewModel.CharacterStatBarViewModel.ManaBarViewModel.MaxValue.Value = value).AddTo(compositeDisposable);
            model.CurrentMana.Subscribe(value => ViewModel.CharacterStatBarViewModel.ManaBarViewModel.CurrentValue.Value = value).AddTo(compositeDisposable);

            
        }
    }
}