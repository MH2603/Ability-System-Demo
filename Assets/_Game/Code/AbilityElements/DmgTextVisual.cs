using UnityEngine;
using Foundation.EntitySystem;

namespace MH.Character
{
    public class DmgTextVisual : EntityComponent
    {

        #region ---------- Inspector ----------------

        [SerializeField] private DynamicTextData DynamicTextData;
        [SerializeField] private float offset = 0.5f;

        #endregion

        
        private CharacterHealth characterHealth => entity.Get<CharacterHealth>();
        private Transform body;
        private Transform camRoot;

        private Vector3 myPos => body.position;

        public override void ManualStart()
        {
            base.ManualStart();


            characterHealth.OnHit += ShowText;
            body = transform;
            camRoot = Camera.main.transform;
        }


        private void ShowText(Vector3 hitDirection, float dmg)
        {
            Vector3 pos = myPos + (camRoot.position - myPos).normalized * offset;
            pos.y = myPos.y;

            DynamicTextManager.CreateText(pos, dmg.ToString("F2"), DynamicTextData);
        }


    }
}
