using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MH.Ability
{
    public class AreaAbilityIndicator : BaseAbilityIndicator
    {

        protected override void ManualUpdate()
        {
            base.ManualUpdate();

            UpdateAreaIndicatorPos();
        }

        public override void Init(Transform entityTransform, float limitRadius = 10, float attackSize = 1)
        {
            base.Init(entityTransform, limitRadius, attackSize);

            dealDmgIndicator.localScale = Vector3.one * attackSize;
        }

        public void UpdateAreaIndicatorPos()
        {
            Vector3 pos = targetPos;
            pos.y = body.position.y;
            dealDmgIndicator.position = pos;
        }

    }
}
