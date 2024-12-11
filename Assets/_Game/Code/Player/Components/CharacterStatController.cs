using System;
using Foundation.EntitySystem;
using Foundation.Extensions.AttributeSystem;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MH.Character
{
    public enum ECharacterStat 
    {
        DMG = 0,
        Defence = 1,
        HP = 2,
        Mana = 3,
        AttackSpeed = 4
    }
    
    public static class CharacterStatToString
    {
        private static readonly Dictionary<ECharacterStat, string> StatTypeStrings = 
            Enum.GetValues(typeof(ECharacterStat))
                .Cast<ECharacterStat>()
                .ToDictionary(stat => stat, stat => stat.ToString());

        public static string Convert(ECharacterStat abilityType)
        {
            return StatTypeStrings[abilityType];    
        }
            
    }

    [System.Serializable]
    public class CharacterStat : IBaseStat
    {
        [field:SerializeField] public ECharacterStat characterStatType { get; private set; }
        public string StatName
        {
            get => CharacterStatToString.Convert(characterStatType);
        }
        [field:SerializeField] public float BaseValue { get; private set; }
    }
    
    public class CharacterStatController : EntityComponent
    {
        [SerializeField] private List<CharacterStat> _baseStats;
        private StatsMediator _statsMediator = new();

        [ShowInInspector]
        private IReadOnlyDictionary<object, List<StatModifier>> ModifiersSourceMap => _statsMediator.ModifiersSourceMap;

        public override void ManualStart()
        {
            base.ManualStart();
            foreach (var baseStat in _baseStats)
            {
                _statsMediator.TryAddStat(baseStat);
            }
        }

        public override void ManualUpdate()
        {
            base.ManualUpdate();
            _statsMediator.ManualUpdate();
        }

        public void AddModifier(StatModifier modifier)
        {
            _statsMediator.AddModifier(modifier);
        }

        public float GetFinalStatValue(string statName)
        {
            return _statsMediator.GetFinalStatValue(statName);
        }

        public float GetFinalStatValue(ECharacterStat statType)
        {
            return GetFinalStatValue(CharacterStatToString.Convert(statType));
        }
    }
}
