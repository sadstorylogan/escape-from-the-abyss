using System;
using Input;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float gravity = -9.81f;
        [SerializeField] private float rotationSpeed = 5f;
        private Vector3 velocity;
        private CharacterController characterController;
        private InputManager inputManager;

        private void Awake()
        {
            characterController = GetComponent<CharacterController>();
            inputManager = GetComponent<InputManager>();
        }

        private void Update()
        {
            var moveDirection = inputManager.MoveInput;
            var move = new Vector3(moveDirection.x, 0, moveDirection.y).normalized;
            
            // Rotate the character to face the move direction
            if (move != Vector3.zero) // Check to avoid setting rotation when there's no input
            {
                var targetAngle = Mathf.Atan2(move.x, move.z) * Mathf.Rad2Deg;
                var targetRotation = Quaternion.Euler(0, targetAngle, 0);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }

            // Apply gravity
            if (characterController.isGrounded && velocity.y < 0)
            {
                velocity.y = -2f; // Small downward force to keep the player grounded
            }
            else
            {
                velocity.y += gravity * Time.deltaTime;
            }

            // Combine horizontal movement with vertical movement due to gravity
            var combinedMove = move * moveSpeed + velocity;

            // Use CharacterController.Move() to move the character
            characterController.Move(combinedMove * Time.deltaTime);
        }
    }
}
