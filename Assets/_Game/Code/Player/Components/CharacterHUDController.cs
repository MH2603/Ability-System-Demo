using Cysharp.Threading.Tasks;
using Foundation.EntitySystem;
using MH.UI;
using MH.UISystem.HUD;
using UniRx;

namespace MH.Character
{
    public class CharacterHUDController : EntityComponent
    {
        
        private MainHUDModel _hudModel = new();  
        private CharacterHealth _characterHealth => entity.Get<CharacterHealth>();
        private CharacterMana _characterMana => entity.Get<CharacterMana>();   

        public override void ManualStart()
        {
            _characterHealth.OnHealthChanged += UpdateHpModel;
            _characterMana.OnManaChanged.Subscribe(currentMana => UpdateManaModel()).AddTo(this);
            
            UpdateHpModel();
            UpdateManaModel();
            HUDBaseLayer.Main.ShowAsync<MainHUD, MainHUDModel>(_hudModel).Forget();
            
        }


        private void UpdateHpModel()
        {
            _hudModel.CurrentHp.Value = _characterHealth.CurrentHealth;
            _hudModel.MaxHp.Value = _characterHealth.MaxHealth; 
        }

        private void UpdateManaModel()
        {
            _hudModel.CurrentMana.Value = _characterMana.CurrentMana;
            _hudModel.MaxMana.Value = _characterMana.MaxMana;   
        }
        
    }
}