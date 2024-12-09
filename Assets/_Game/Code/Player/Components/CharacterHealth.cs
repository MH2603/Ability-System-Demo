using Foundation.EntitySystem;
using Sirenix.OdinInspector;
using System;
using UnityEngine;

namespace MH.Character
{
    public interface IDamageable
    {
        void TakeDamage(EntityBase attacker, float damage);
    }

    public class CharacterHealth : EntityComponent, IDamageable
    {
        #region Inspector

        [SerializeField] private float _currentHealth;
        [SerializeField] private float _maxHealth;

        #endregion

        //private CharacterWorldSpaceUIController WorldSpaceUIController => entity.Get<CharacterWorldSpaceUIController>();
        private CharacterStat CharacterStat => entity.Get<CharacterStat>();

        public float HealthPercentage => _currentHealth / _maxHealth;
        public bool IsDead => Mathf.Approximately(_currentHealth, 0);

        public float CurrentHealth
        {
            get => _currentHealth;
            set => _currentHealth = Mathf.Clamp(value, 0, _maxHealth);
        }
        
        public float MaxHealth => _maxHealth;

        public event Action OnDeath;
        public event Action OnHealthChanged;
        public event Action OnRevive;
        public event Action<Vector3, float> OnHit;

        public override void ManualStart()
        {
            base.ManualStart();
            _currentHealth = _maxHealth = CharacterStat.GetFinalStatValue("HP");
            //entity.ServiceLocator.RegisterAsImplementedInterfaces(this);
        }

        public override void ManualUpdate()
        {
            base.ManualUpdate();
            UpdateHealthBarUI();
            if (IsDead)
            {
                OnDeath?.Invoke();
            }
        }

        /// <summary>
        /// Revives the entity to either full health or a minimum specified health value.
        /// </summary>
        /// <param name="fullHealth">Flag indicating whether to revive the entity to full health.</param>
        /// <param name="minHealth">The minimum health value to which the entity should be revived if fullHealth is false. Default is 1.</param>
        [Button("Revive", ButtonSizes.Medium, Expanded = false)]
        public void Revive(bool fullHealth = false, float minHealth = 1)
        {
            if (!IsDead)
            {
                Debug.LogWarning("Character is not dead. Revive method should only be called when the entity is dead.");
                return;
            }

            _currentHealth = fullHealth ? _maxHealth : minHealth;
            OnRevive?.Invoke();
        }

        /// <summary>
        /// Inflicts damage on the entity by reducing its current health based on the specified damage value.
        /// </summary>
        /// <param name="attacker">The entity inflicting the damage.</param>
        /// <param name="damage">The amount of damage to be inflicted on the entity.</param>
        [Button("Take Damage", ButtonSizes.Medium, Expanded = false)]
        public void TakeDamage(EntityBase attacker, float damage)
        {
            if (IsDead)
            {
                Debug.LogWarning("Character is already dead. Cannot take damage.");
                return;
            }

            CurrentHealth -= damage;
            var hitDirection = transform.position - attacker.transform.position;

            // raise actions
            OnHit?.Invoke(hitDirection, damage);
            OnHealthChanged?.Invoke();
        }

        private void UpdateHealthBarUI()
        {
            // We hide the health bar if the health is full
            //WorldSpaceUIController.SetActiveHpBar(!Mathf.Approximately(HealthPercentage, 1));
            //WorldSpaceUIController.SetHpBarValue(HealthPercentage);
        }
    }
}
