using UnityEngine;

namespace MH.Ability
{
    public interface IAbilityIndicator
    {
        void Init(Transform entityTransform, float limitRadius, float attackSize);
        void SetTargetPos(Vector3 targetPos);
        void Off();

        Vector3 GetTargetPos();
    }

    public abstract class BaseAbilityIndicator : MonoBehaviour, IAbilityIndicator
    {
        #region ----------- Inspector -------------
        [SerializeField] protected bool showAttackLimit;
        
        [SerializeField] protected Transform attackLimitIndicator;
        [Space]
        [SerializeField] protected bool showDmgArea;
        [SerializeField] protected Transform dealDmgIndicator;

        #endregion

        protected Vector3 targetPos;
        protected Transform entityTransform;
        protected Transform body;

        protected float toTargetDistance => Vector3.Distance(entityTransform.position, targetPos);   
        protected Vector3 toTargetDirection => targetPos - entityTransform.position;

        #region ---------- Unity Methods -----------
        private void Awake()
        {
            SetUp();
        }

        private void Update()
        {
            ManualUpdate();
        }

        #endregion

        public virtual void Init(Transform entityTransform, float limitRadius = 10, float attackSize=1)
        {
            this.entityTransform = entityTransform;

            attackLimitIndicator.gameObject.SetActive(showAttackLimit);
            dealDmgIndicator.gameObject.SetActive(showDmgArea); 


            attackLimitIndicator.transform.localScale = Vector3.one * limitRadius;

            gameObject.SetActive(true);
        }

        public virtual void SetTargetPos(Vector3 targetPos)
        {
            this.targetPos = targetPos;
        }

        public Vector3 GetTargetPos()
        {
            return this.targetPos;  
        }

        public void Off()
        {
            gameObject.SetActive(false);
        }

        protected virtual void SetUp()
        {
            transform.SetParent(null);
            body = transform;
        }

        protected virtual void ManualUpdate()
        {
            body.position = entityTransform.position;  
        }

        protected virtual void ManualDeactive()
        {

        }

        
    }
}
