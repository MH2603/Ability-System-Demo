using Cysharp.Threading.Tasks;
using Foundation.EntitySystem;
using MH.Ability;
using MH.UI;
using MH.UI.Ability;
using MH.UI.Ability.RuntimeStats;
using MH.UISystem.HUD;
using UniRx;
using UnityEngine;

namespace MH.Character
{
    public class CharacterHUDController : EntityComponent
    {

        #region  ---------- Inspectors ----------

        [SerializeField] private ECharacterStat[] _needShowCharacterStats;

        #endregion

        #region ----- Properties ------------

        private MainHUDModel _hudModel = new();  
        private CharacterHealth _characterHealth => entity.Get<CharacterHealth>();
        private CharacterMana _characterMana => entity.Get<CharacterMana>();  
        private CharacterAbilityManager _abilityManager => entity.Get<CharacterAbilityManager>();
        private CharacterStatController _characterStatCtrl => entity.Get<CharacterStatController>();

        #endregion
        

        public override void ManualStart()
        {
            SetUpBarStats();
            SetUpAbility();
            SetUpRuntimeStats();
            
            HUDBaseLayer.Main.ShowAsync<MainHUD, MainHUDModel>(_hudModel).Forget();
            
        }

        #region  ------ Hp and Mana Bar --------------

        private void SetUpBarStats()
        {
            _characterHealth.OnHealthChanged += UpdateHpModel;
            _characterMana.OnManaChanged.Subscribe(currentMana => UpdateManaModel()).AddTo(this);
            
            UpdateHpModel();
            UpdateManaModel();
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
        
        #endregion

        #region  ------- Ability ----------

        private void SetUpAbility()
        {
            foreach (var ability in _abilityManager.Abilities)
            {
                
                string abilityName  = ability.Name;
                BaseAbilitySO config = _abilityManager.GetAbilityConfig(abilityName);
                
                HotBarAbilityViewModel abilityViewModel = new();
                abilityViewModel.AbilityName.Value = abilityName;
                abilityViewModel.AbilityIcon.Value = config.AbilityAvatar;
                abilityViewModel.KeyCode.Value = config.InputConfig.primaryKey;

                ability.OnEnter += () =>
                {
                    abilityViewModel.IsCasting.Value = true;
                };
                
                ability.OnExit += () =>
                {
                    abilityViewModel.IsCasting.Value = false;
                    abilityViewModel.IsCoolingDown.Value = true;
                    abilityViewModel.CoolDownDuration.Value = ability.Cooldown;
                };
                
                ability.RemainingCooldown.Subscribe(remainingCoolDown =>
                {
                    abilityViewModel.RemainCoolDown.Value = remainingCoolDown;
                    // Debug.Log($" Update abilityViewModel.RemainCoolDown = {abilityViewModel.RemainCoolDown.Value} !");
                    if(ability.IsReady) abilityViewModel.IsCoolingDown.Value = false;
                }).AddTo(this);

                _hudModel.AbilityVModelMap[abilityName] = abilityViewModel;
            }
        }

        #endregion


        #region  ----------- Runtime Stats ------------

        private void SetUpRuntimeStats()
        {
            RuntimeStatViewModel runtimeStatViewModel = null;
            foreach (var statType in _needShowCharacterStats)
            {
                runtimeStatViewModel = new();
                RegisterRuntimeStatVModel(runtimeStatViewModel, statType);
                
                _hudModel.StatVModelMap[CharacterStatToString.Convert(statType)] = runtimeStatViewModel;    
            }
        }

        private void RegisterRuntimeStatVModel(RuntimeStatViewModel viewModel, ECharacterStat statType)
        {
            viewModel.StatName.Value = CharacterStatToString.Convert(statType);
            Observable.EveryUpdate().
                        Where( _=> (int)viewModel.StatValue.Value != (int)_characterStatCtrl.GetFinalStatValue(statType)  ).
                        Subscribe(_=> viewModel.StatValue.Value = _characterStatCtrl.GetFinalStatValue(statType)).
                        AddTo(this);
            
        }
        
        #endregion
    }
}