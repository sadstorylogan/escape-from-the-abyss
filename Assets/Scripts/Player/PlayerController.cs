using System;
using Input;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private float moveSpeed = 5.0f;
        [SerializeField] private float gravity = -9.81f;
        
        // Velocity vector to store vertical movement due to gravity
        private Vector3 velocity; 
        private CharacterController characterController;
        private InputManager inputManager;

        private void Awake()
        {
            inputManager = GetComponent<InputManager>();
            characterController = GetComponent<CharacterController>();
        }

        private void Update()
        {
            //Get the movement input from the InputManager script
            var moveDirection = new Vector3(inputManager.MoveInput.x, 0, inputManager.MoveInput.y).normalized;
            
            // Apply gravity (if the character is not grounded)
            if (!characterController.isGrounded)
            {
                velocity.y += gravity * Time.deltaTime;
            }
            else if (characterController.isGrounded && velocity.y < 0)
            {
                // Small downward force to keep the player grounded
                velocity.y = -2f; 
            }
            
            // Combine horizontal movement with vertical movement due to gravity
            var move = moveDirection * moveSpeed + velocity;

            // Use CharacterController.Move() to move the character
            characterController.Move(move * Time.deltaTime);
        }
    }
}
