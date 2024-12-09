using UnityEngine;

namespace MH.Ability
{
    public class DirectionAbilityIndicator : BaseAbilityIndicator
    {
        #region ---------- Inspector -------------

        //[SerializeField] private Transform castIndicator;


        #endregion

        protected override void ManualUpdate()
        {
            base.ManualUpdate();

            SetDirction();
            SetScaleBasedOnTargetPos();
        }

        

        private void SetScaleBasedOnTargetPos()
        {
            Vector3 scale = dealDmgIndicator.localScale;
            scale.z = toTargetDistance;
            dealDmgIndicator.localScale = scale;
        }

        private void SetDirction()
        {
            Vector3 direction = toTargetDirection;
            direction.y = 0;
            dealDmgIndicator.rotation = Quaternion.LookRotation(direction);
        }
    }
}
