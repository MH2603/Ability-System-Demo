using Foundation.EntitySystem;
using UnityEngine;

namespace MH.Character
{
    public class CharacterStateMachineController : EntityComponent
    {
        [SerializeField] private ECharacterState state;
        private CharacterStateMachine _stateMachine;

        public override void ManualStart()
        {
            base.ManualStart();
            _stateMachine = new CharacterStateMachine(entity);
        }

        public override void ManualUpdate()
        {
            base.ManualUpdate();
            _stateMachine.Update();

            state = _stateMachine.CurrentState;
        }

        public void TransitionToState(ECharacterState state)
        {
            _stateMachine.TransitionTo(state);
        }
    }
}
