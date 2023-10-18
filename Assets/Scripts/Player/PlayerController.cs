using System;
using System.Collections;
using Input;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class PlayerController : MonoBehaviour
    {
        // Movement variables
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float gravity = -9.81f;
        [SerializeField] private float rotationSpeed = 5f;
        private Vector3 velocity;
        
        // Dash mechanics variables
        [SerializeField] private float dashSpeed = 10f;
        [SerializeField] private float dashDuration = 1f;
        [SerializeField] private float dashCooldown = 2f;
        [SerializeField] private float lastDashTime = -10f;
        private bool isDashing;
        
        // Attack variables
        private int comboStage = 0;
        private float lastAttackTime;
        private float comboResetTime = 1f;
        private bool isAttacking = false;
        
        private CharacterController characterController;
        private InputManager inputManager;
        private Animator animator;
        
        private void Awake()
        {
            characterController = GetComponent<CharacterController>();
            inputManager = GetComponent<InputManager>();
            animator = GetComponent<Animator>();
        }

        private void Start()
        {
            inputManager.OnAttack += Attack;
        }

        private void Attack()
        {
            // If not already attacking, start a new attack sequence.
            if (!isAttacking)
            {
                isAttacking = true;
                comboStage = 1; // Start at the first stage of the combo.
                animator.SetTrigger("Attack");
                animator.SetInteger("Combo", comboStage);
                lastAttackTime = Time.time; // Record the time of this initial attack.
            }
            else if (Time.time - lastAttackTime <= comboResetTime) // We are within the combo continuation window.
            {
                // If we're not at the last stage of the combo, advance to the next stage.
                if (comboStage < 3)
                {
                    comboStage++;
                    animator.SetInteger("Combo", comboStage);
                    lastAttackTime = Time.time; // Update the time of the last attack.
                }
            }
            
            Debug.Log("Attack button pressed. Combo stage: " + comboStage);
        }

        private void Update()
        {
            // If we're in an attack and too much time has passed since the last attack in the combo, reset.
            if (isAttacking && Time.time - lastAttackTime > comboResetTime)
            {
                isAttacking = false;
                comboStage = 0;
                animator.SetInteger("Combo", comboStage); // Reset the combo in the animator.
            }
            
            if (!isDashing)
            {
                HandleMovement();
            }
            
            // Dash handling
            if (inputManager.DashRequested && Time.time >= lastDashTime + dashCooldown)
            {
                StartCoroutine(PerformDash());
            }
        }

        private void OnDestroy()
        {
            inputManager.OnAttack -= Attack;
        }

        private IEnumerator PerformDash()
        {
            isDashing = true; 
            inputManager.DisableMovementInput();
            lastDashTime = Time.time;
            
            animator.SetTrigger("Dash");
            
            var dashEndTime = Time.time + dashDuration;
            while (Time.time > dashEndTime)
            {
                var dashDirection = transform.forward; // The character dashes in the direction they are facing
                characterController.Move(dashDirection * (dashSpeed * Time.deltaTime));
                yield return null; // Wait until next frame
            }
            
            inputManager.DashRequested = false;
            yield return new WaitForSeconds(dashDuration);
            
            inputManager.EnableMovementInput();
            isDashing = false;
        }

        private void HandleMovement()
        {
            // Fetch the movement input from the InputManager
            var moveDirection = inputManager.MoveInput;

            // Convert the 2D input into a 3D vector for movement
            var move = new Vector3(moveDirection.x, 0, moveDirection.y).normalized;

            // Update the Animator Parameter
            var moveMagnitude = moveDirection.magnitude;
            animator.SetFloat("Speed", moveMagnitude);

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
