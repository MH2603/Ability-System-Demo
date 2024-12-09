using System;
using UnityEngine;

namespace MH.Ability
{
    public enum EBuffType
    {
        Buff,
        DeBuff
    }

    public enum EModifierOperation
    {
        Plus,
        Multi
    }


    [System.Serializable]
    public class BuffStat : AbilityStat
    {
        [field: SerializeField] public string AffectStatName { get; private set; }
        [field: SerializeField] public EBuffType BuffType { get; private set; }
        [field: SerializeField] public EModifierOperation OperationType { get; private set; }
    }

    [Serializable]
    [CreateAssetMenu(fileName = "Buff Ability", menuName = "Ability/Create Ability/Buff Ability")]
    public class BuffAilityConfigSO : BaseAbilitySO
    {
        [field: SerializeField] public BuffStat[] BuffStats { get; private set; }
        [field: SerializeField] public GameObject BuffVfxPrefab { get; private set; }

        public override IAbility CreateAbility()
        {
            BuffAbility buffAbility = new BuffAbility(this);

            return buffAbility;
        }
    }
}
