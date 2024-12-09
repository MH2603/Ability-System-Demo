using MH.Projectile;
using MH.Skill;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MH.Ability
{
    [Serializable]
    [CreateAssetMenu(fileName = "Fire Ball", menuName = "Ability/Create Ability/Fire Ball")]
    public class FireBallConfigSO : BaseAbilitySO
    {
        [field: SerializeField] public BaseProjectile ProjectilePrefab;
        [field: SerializeField] public LayerMask TargetLayer;

        public override IAbility CreateAbility()
        {
            FireBallAbility ability = new FireBallAbility(this);

            return ability;
        }

        
    }
}
