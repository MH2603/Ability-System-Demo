using Foundation.EntitySystem;
using Foundation.Extensions.Utilities.StateMachine;

namespace MH.Character
{
    public enum ECharacterState
    {
        Locomotion,
        Attack, // not use
        Jump, // not use
        Dash, // not use
        TakeHit, // not use
        Dead, // not use
        CastSpell // use when skill will controll characrer
    }


    public class CharacterStateMachine : StateMachine<ECharacterState>
    {
        public CharacterStateMachine(EntityBase entityBase)
        {
            AddState(ECharacterState.Locomotion, new CharacterLocomotionState(entityBase, this));
            //AddState(ECharacterState.Attack, new CharacrerNormalAttackState(entityBase, this));
            //AddState(ECharacterState.Attack, new CharacterAttackState(entityBase, this));
            //AddState(ECharacterState.Jump, new CharacterJumpState(entityBase, this));
            //AddState(ECharacterState.Dash, new CharacterDashState(entityBase, this));
            //AddState(ECharacterState.Dead, new CharacterDeadState(entityBase, this));
            //AddState(ECharacterState.TakeHit, new CharacterTakeHitState(entityBase, this));

            StartFsm(ECharacterState.Locomotion);
        }

    }

}
