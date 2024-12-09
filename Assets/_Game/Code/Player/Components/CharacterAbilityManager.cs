using Foundation.EntitySystem;
using MH.Ability;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace MH.Character
{

    public class CharacterAbilityManager : EntityComponent
    {
        #region -------------------- Inspector -------------------

        // This is list ability which will be used in game
        // if player want to use specific ability -> need to drag it's config into here
        // Ability Class will be init in Start Method
        [field: SerializeField] private BaseAbilitySO[] _abilityConfigs { get; set; }

        #endregion

        private Dictionary<string, IAbility> _abilityMap = new();
        private Dictionary<string, BaseAbilitySO> _abilityConfigMap = new();

        public Action OnAbilityEnter;
        public Action OnAbilityExit;


        #region ------------ Entity Methods --------------

        public override void ManualStart()
        {
            base.ManualStart();

            SetUpAbilities(); 
        }

        // ReSharper disable Unity.PerformanceAnalysis
        public override void ManualUpdate()
        {
            base.ManualUpdate();
            foreach (var ability in _abilityMap.Values)
            {
                ability.UpdateAbility();
            }
        }

        #endregion

        #region ------------- Public Methods ---------------

        public void CastAbility(string abilityName, AbilityContext context)
        {
            if (IsAbilityExist(abilityName)) _abilityMap[abilityName].Execute(context);
            else
            {
                Debug.LogWarning($" BUG Ability Execute: Not found abilty with name = {abilityName} to execute ! ");
            }
        }

        public bool IsAbilityReady(string abilityName)
        {
            IAbility needExecuteAbility = GetAbility(abilityName);  

            return needExecuteAbility != null ? needExecuteAbility.IsReady : false;   
        }

        public BaseAbilitySO[] GetAllAbilityConfig()
        {
            return _abilityConfigs;
        }

        public BaseAbilitySO GetAbilityConfig(string abilityName)
        {
            return null;  
        }

        public IAbility GetAbility(string abilityName)
        {
            if (!_abilityMap.ContainsKey(abilityName))
            {
                Debug.LogWarning($" BUG Ability Manager: Not found ability with name = {abilityName}");
                return null;
            }

            return _abilityMap[abilityName];
        }

        public bool IsAbilityExist(string abilityName)
        {
            return _abilityMap.ContainsKey(abilityName);
        }

        #endregion

        #region ------------- Private Methods ---------------

        private void SetUpAbilities()
        {
            foreach (var abilityConfig in _abilityConfigs)
            {
                IAbility newAbility = abilityConfig.CreateAbility();
                newAbility.Initialize(entity);
                _abilityMap[abilityConfig.AbilityName] = newAbility;

                newAbility.OnEnter += OnAbilityEnter;
                newAbility.OnExit += OnAbilityExit;

                _abilityConfigMap[abilityConfig.AbilityName] = abilityConfig;
            }
        }

        #endregion
    }

}
