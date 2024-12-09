using Foundation.EntitySystem;
using MH.Character;
using UnityEngine;

namespace MH
{

    public class CharacterExecuteAbilityState : CharacterBaseState
    {
        public CharacterExecuteAbilityState(EntityBase entity, CharacterStateMachine stateMachine) : base(entity, stateMachine)
        {
            
        }

        public override void OnStateStart()
        {
            base.OnStateStart();


        }

        public override void OnStateExit()
        {
            base.OnStateExit();

            StateMachine.TransitionTo(ECharacterState.Locomotion);
        }
    }

}
