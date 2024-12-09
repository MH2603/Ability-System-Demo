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
    }
    
    public class MainHUD : HUDContext<MainHUDModel, MainHUDView, MainHUDViewModel>
    {
        protected override void OnShowing(MainHUDModel model)
        {
            model.MaxHp.Subscribe(maxValue => ViewModel.characterStatModelView.HpBarViewModel.MaxValue.Value = maxValue).AddTo(compositeDisposable);
            model.CurrentHp.Subscribe(value => ViewModel.characterStatModelView.HpBarViewModel.CurrentValue.Value = value).AddTo(compositeDisposable);  
            model.MaxMana.Subscribe(value => ViewModel.characterStatModelView.ManaBarViewModel.MaxValue.Value = value).AddTo(compositeDisposable);
            model.CurrentMana.Subscribe(value => ViewModel.characterStatModelView.ManaBarViewModel.CurrentValue.Value = value).AddTo(compositeDisposable);
        }
    }
}