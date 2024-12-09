using Foundation.EntitySystem;
using UniRx;

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
        private CharacterStat _characterStat;

        public float CurrentMana => _currentMana;
        public float MaxMana => _characterStat.GetFinalStatValue("Mana");

        public Subject<float> OnManaChanged = new();
        
        #region  ---------- Entity Methods -----------

        public override void ManualStart()
        {
            _characterStat = entity.Get<CharacterStat>();
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
        }
    }
}