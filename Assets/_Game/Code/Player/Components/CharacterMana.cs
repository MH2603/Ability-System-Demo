using Foundation.EntitySystem;
using UniRx;
using UnityEngine;

namespace MH.Character
{
    public interface ICharacterMana
    {
        float CurrentMana { get;  }
        float MaxMana { get; }
        bool CanUseMana(float wantUseValue);
        void UseMana(float wantUseValue);
    }
    
    public class CharacterMana : EntityComponent, ICharacterMana
    {
        private float _currentMana;
        private CharacterStatController _characterStatController;

        public float CurrentMana => _currentMana;
        public float MaxMana => _characterStatController.GetFinalStatValue("Mana");

        public Subject<float> OnManaChanged = new();
        
        #region  ---------- Entity Methods -----------

        public override void ManualStart()
        {
            _characterStatController = entity.Get<CharacterStatController>();
            _currentMana = MaxMana;
        }

        #endregion

        public bool CanUseMana(float wantUseValue)
        {
            return _currentMana >= wantUseValue;
        }

        public void UseMana(float wantUseValue)
        {
            if(!CanUseMana(wantUseValue)) return;   
            
            _currentMana -= wantUseValue;
            _currentMana = Mathf.Clamp(_currentMana, 0, MaxMana);   
        }
    }
}