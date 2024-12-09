using MH.Ability;
using MH.Projectile;
using System;
using UnityEngine;

namespace MH.Skill.Archer
{
    [Serializable]
    [CreateAssetMenu(fileName = "Range Normal Attack", menuName = "Ability/Archer/ Normal Attack")]
    public class RangeNormalAttackSO : BaseAbilitySO
    {
        #region -------------------- Inspector -------------------
        [field: SerializeField] public LayerMask EnemyLayer { get; private set; }
        [field: SerializeField] public BaseProjectile ProjectilePrefab { get; private set; }

        public override IAbility CreateAbility()
        {
            throw new NotImplementedException();
        }
        #endregion

        public BaseProjectile CreateNewProjectile()
        {
            var newProjectile = Instantiate(ProjectilePrefab);
            return newProjectile;   
        }

    }

}
