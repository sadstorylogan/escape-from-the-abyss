using System;
using UnityEngine;

namespace Input
{
    public class InputManager : MonoBehaviour
    {
        private PlayerControls playerControls;
        public Vector2 MoveInput { get; private set; }

        private void Awake()
        {
            playerControls = new PlayerControls();
            
            // Bind the move action to a callback
            playerControls.Gameplay.Move.performed += ctx => MoveInput = ctx.ReadValue<Vector2>();
            playerControls.Gameplay.Move.canceled += ctx => MoveInput = Vector2.zero;
        }

        private void OnEnable()
        {
            playerControls.Gameplay.Enable();
        }

        private void OnDisable()
        {
            playerControls.Gameplay.Disable();
        }
    }
}
