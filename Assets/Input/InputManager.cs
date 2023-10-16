using System;
using UnityEngine;

namespace Input
{
    public class InputManager : MonoBehaviour
    {
        private PlayerControls playerControls;
        public Vector2 MoveInput { get; private set; }
        public bool DashRequested { get;  set; }

        private void Awake()
        {
            playerControls = new PlayerControls();
            
            // Bind the move action to a callback
            playerControls.Gameplay.Move.performed += ctx => MoveInput = ctx.ReadValue<Vector2>();
            playerControls.Gameplay.Move.canceled += ctx => MoveInput = Vector2.zero;

            playerControls.Gameplay.Dash.performed += ctx => DashRequested = true;
            playerControls.Gameplay.Dash.canceled += ctx => DashRequested = false;
        }

        private void OnEnable()
        {
            playerControls.Gameplay.Enable();
        }

        private void OnDisable()
        {
            playerControls.Gameplay.Disable();
        }
        
        public void DisableMovementInput()
        {
            playerControls.Gameplay.Move.Disable();
        }

        public void EnableMovementInput()
        {
            playerControls.Gameplay.Move.Enable();
        }
    }
}
