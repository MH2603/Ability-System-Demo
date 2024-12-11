using Foundation.EntitySystem;
using Foundation.Extensions.AttributeSystem;
using MH.Ability;
using MH.Character;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

namespace MH.Ability
{
    
    public enum EAbilityStat
    {
        None,
        Dmg,
        CoolDown,
        ManaCost,
        AttackRadius,
        ProjectileSpeed,
        ProjectileRotateSpeed,
        DmgRadius,
        DmgDuration,
        
        Buff
    }

    public static class AbilityStatTypeString
    {
        private static readonly Dictionary<EAbilityStat, string> StatTypeStrings = 
            Enum.GetValues(typeof(EAbilityStat))
                .Cast<EAbilityStat>()
                .ToDictionary(stat => stat, stat => stat.ToString());

        public static string Convert(EAbilityStat abilityType)
        {
            return StatTypeStrings[abilityType];    
        }
            
    }

    [Serializable]
    public class AbilityStat : IBaseStat
    {
        [FormerlySerializedAs("StatType")] [SerializeField] private EAbilityStat statType;
        [SerializeField] public string StatName { get => statType.ToString(); }
        [field: SerializeField] public float BaseValue { get; private set; }
    }

    [Serializable]
    //[CreateAssetMenu(fileName = "Base Ability", menuName = "Ability/Create Base Ability")]
    public abstract class BaseAbilitySO : ScriptableObject
    {
        #region ---------- Inspector --------------
        
        [field: SerializeField] public string AbilityName { get; private set; }
        [field: SerializeField] public Sprite AbilityAvatar { get; private set; }
        [field: SerializeField] public AbilityStat[] Stats { get; private set; }

        [field: SerializeField] public AbilityInputConfig InputConfig { get; private set; }
        #endregion

        public abstract IAbility CreateAbility();
        

    }

   

}
