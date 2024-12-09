using UnityEngine;
using Foundation.EntitySystem;
using System;

namespace MH.Character
{

    public class PlayerInputController : EntityComponent
    {
        public Action<Vector2> OnMove;
        public Vector2 MoveInput => inputActions.Player.Move.ReadValue<Vector2>();

        private PlayerInput inputActions;

        public override void ManualStart()
        {
            base.ManualStart();

            inputActions = new PlayerInput();
            inputActions.Enable();
        }

        public override void ManualUpdate()
        {
            base.ManualUpdate();

            CheckMoveInput();
        }

        private void CheckMoveInput()
        {
            if (inputActions.Player.Move.WasPressedThisFrame())
            {
                OnMove?.Invoke(inputActions.Player.Move.ReadValue<Vector2>());
            }
        }
    }

}
