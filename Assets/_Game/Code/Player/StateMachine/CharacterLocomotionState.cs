using Foundation.EntitySystem;
using UnityEngine;

namespace MH.Character
{
    public class CharacterLocomotionState : CharacterBaseState
    {

        private Vector3 targetPoint;
        private Transform entityTransform;
        private bool isMoving = false;  
        private float distance => Vector3.Distance(targetPoint, entityTransform.position);
        private Vector2 directionMove;

        public CharacterLocomotionState(EntityBase entity, CharacterStateMachine stateMachine) :
            base(entity, stateMachine)
        {
            entityTransform = entity.transform;
            MouseRaycaster.OnRightClick += OnSetTarget;
        }

        //private CharacterDetector CharacterDetector => entity.Get<CharacterDetector>();
        //private CharacterTargetController CharacterTargetController => entity.Get<CharacterTargetController>();

        //private void OnDetect(bool isInRage, GameObject target)
        //{
        //    if (isInRage)
        //        CharacterTargetController.SetTarget(target.transform);
        //    else
        //        CharacterTargetController.ClearTarget();
        //}

        public override void OnStateStart()
        {
            base.OnStateStart();
        }

        public override void OnStateEnter()
        {
            base.OnStateEnter();
            CharacterAnimationController.PlayAnimation("Locomotion");

            //CharacterInputController.OnAttack += OnAttack;
            //CharacterInputController.OnJump += OnJump;
            //CharacterInputController.OnDash += OnDash;
            //CharacterDetector.OnDetect += OnDetect;

            targetPoint = entityTransform.position;
            isMoving = false;
        }

        //private void OnDash()
        //{
        //    StateMachine.TransitionTo(ECharacterState.Dash);
        //}

        //private void OnJump()
        //{
        //    StateMachine.TransitionTo(ECharacterState.Jump);
        //}

        //private void OnAttack()
        //{
        //    StateMachine.TransitionTo(ECharacterState.Attack);
        //}

        private void OnSetTarget(ClickEventData clickData)
        {
            if(clickData.GameObject.layer != LayerMask.NameToLayer("Ground"))
            {
                return;
            }

            targetPoint = clickData.Position;
            isMoving = true;

            //Debug.Log($" Locomotion State: Set Target : {targetPoint} - {isMoving} - {distance}");
            Vector3 direction = targetPoint - entityTransform.position;
            directionMove = new Vector2(direction.x, direction.z);
        }

        public override void OnStateExit()
        {
            base.OnStateExit();

            //CharacterInputController.OnAttack -= OnAttack;
            //CharacterInputController.OnJump -= OnJump;
            //CharacterInputController.OnDash -= OnDash;
            //CharacterDetector.OnDetect -= OnDetect;
        }

        public override void OnStateUpdate()
        {
            base.OnStateUpdate();

            CheckToMoveToTarget();
            CharacterAnimationController.SetFloatParamater("Speed", isMoving ? 1 : 0);
        }

        private void CheckToMoveToTarget()
        {
            if (!isMoving) return;

            if (distance <= 0.5f)
            {
                isMoving = false;
                return;
            }

            CharacterMovement.Move(directionMove);
        }
    }
}
