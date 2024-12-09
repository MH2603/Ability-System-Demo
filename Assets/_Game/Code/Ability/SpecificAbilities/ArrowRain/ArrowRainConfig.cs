using System;
using UnityEngine;

namespace MH.Ability
{
    [Serializable]
    [CreateAssetMenu(fileName = "Arrow Rain", menuName = "Ability/Create Ability/Arrow Rain")]
    public class ArrowRainConfig : BaseAbilitySO
    {
        [field: SerializeField] public AreaDamageInstance AreaDamagePrefab { get; private set; }
        [field: SerializeField] public LayerMask TargetLayer { get; private set; }

        public override IAbility CreateAbility()
        {
            IAbility ability = new ArrowRainAbility(this);
            return ability;
        }
    }
}
