using Foundation.EntitySystem;
using Foundation.Extensions.AttributeSystem;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

namespace MH.Character
{
    public class CharacterStat : EntityComponent
    {
        [SerializeField] private List<BaseStat> _baseStats;
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
    }
}
