using Foundation.EntitySystem;
using Foundation.Extensions.Utilities.StateMachine;
using UnityEngine;

namespace MH.Character
{
    public abstract class CharacterBaseState : BaseState<ECharacterState, CharacterStateMachine>
    {
        protected EntityBase entity;
        protected ECharacterState previousState;

        protected CharacterAnimationController CharacterAnimationController => entity.Get<CharacterAnimationController>();
        protected AnimationEventController AnimationEvent => entity.Get<AnimationEventController>();

        protected CharacterMovement CharacterMovement => entity.Get<CharacterMovement>();

        //protected CharacterInputController CharacterInputController => entity.Get<CharacterInputController>();
        protected PlayerInputController CharacterInputController => entity.Get<PlayerInputController>();
        protected PlayerMouseRaycaster MouseRaycaster => entity.Get<PlayerMouseRaycaster>();
        //protected CharacterHealth CharacterHealth => entity.Get<CharacterHealth>();

        protected CharacterAbilityManager CharacterAbility => entity.Get<CharacterAbilityManager>();
        protected PointContainer PointContainer => entity.Get<PointContainer>();    
        protected CharacterStat CharacterStat => entity.Get<CharacterStat>();   

        protected CharacterBaseState(EntityBase entity, CharacterStateMachine stateMachine) : base(stateMachine)
        {
            this.entity = entity;
        }

        public override void OnStateStart()
        {
            base.OnStateStart();

            CharacterAbility.OnAbilityEnter += OnExcuteAbility;
        }

        public override void OnStateEnter()
        {
            base.OnStateEnter();
            //CharacterHealth.OnDeath += OnDeath;
            //CharacterHealth.OnHit += OnHit;
        }

        protected virtual void OnHit(Vector3 hitDir)
        {
            if (StateMachine.CurrentState is ECharacterState.Dead) return;
            previousState = StateMachine.CurrentState;

            //if (CharacterAnimationController.IsTakeHitAddictive)
            //    CharacterAnimationController.PlayAddictiveAnimation(EAnimationType.Hit);
            //else
            //{
            //    StateMachine.TransitionTo(ECharacterState.TakeHit);
            //}
            CharacterMovement.PushBack(hitDir);
        }

        protected virtual void OnExcuteAbility()
        {
            if (StateMachine.CurrentState is ECharacterState.Dead) return;

            StateMachine.TransitionTo(ECharacterState.CastSpell);
        }

        private void OnDeath()
        {
            if (StateMachine.CurrentState != ECharacterState.Dead)
                StateMachine.TransitionTo(ECharacterState.Dead);
        }

        public override void OnStateExit()
        {
            base.OnStateExit();
            //CharacterHealth.OnDeath -= OnDeath;
            //CharacterHealth.OnHit -= OnHit;
        }
    }
}
