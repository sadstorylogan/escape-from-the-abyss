using Player;
using UnityEngine;
using UnityEngine.AI;

namespace Enemy
{
    public class EnemyAI : MonoBehaviour
    {
        private enum State
        {
            Idle,
            Chase,
            Attack,
            Hit,
            Dead
        }

        private State currentState = State.Idle;
        
        [Header("References")]
        [SerializeField] private Transform player;
        private Animator animator;
        private NavMeshAgent navMeshAgent;
        private DamageFeedback damageFeedback; 
    
        [Header("AI Properties")]
        [SerializeField] private float detectionRange = 5f;
        [SerializeField] private float attackRange = 1f;
        [SerializeField] private float attackDamage = 10f;
        [SerializeField] private float attackDelay = 1f; // Time delay between successive attacks
        private float lastAttackTime;
        
        [Header("Health Properties")]
        [SerializeField] private float maxHealth = 100f;
        private float currentHealth;

        private void Awake()
        {
            animator = GetComponent<Animator>();
            navMeshAgent = GetComponent<NavMeshAgent>();
            damageFeedback = GetComponentInChildren<DamageFeedback>();

            if (damageFeedback == null)
            {
                Debug.LogError("No DamageFeedback script found on the children object" + gameObject.name);
            }
            
            currentHealth = maxHealth;
        }
    
        private void Update()
        {
            var distanceToPlayer = Vector3.Distance(transform.position, player.position);

            switch (currentState)
            {
                case State.Idle:
                    navMeshAgent.isStopped = true;
                    if (distanceToPlayer <= detectionRange)
                    {
                        currentState = State.Chase;
                    }
                    break;
                case State.Chase:
                    navMeshAgent.isStopped = false;
                    navMeshAgent.SetDestination(player.position);
                    if (distanceToPlayer <= attackRange)
                    {
                        currentState = State.Attack;
                    }
                    else if (distanceToPlayer > detectionRange)
                    {
                        currentState = State.Idle;
                    }
                    break;

                case State.Attack:
                    navMeshAgent.isStopped = true;
                    // Here you can add logic to make the enemy attack the player.
                    // For example, reducing the player's health.
                    if (Time.time - lastAttackTime >= attackDelay)
                    {
                        player.GetComponent<PlayerController>().TakeDamage(attackDamage);
                        lastAttackTime = Time.time;
                    }
                    if (distanceToPlayer > attackRange)
                    {
                        currentState = State.Chase;
                    }
                    break;
                
                case State.Hit:
                    // Logic for hit state can be handled here if needed
                    break;
            }
            HandleAnimations();
        }
    
        private void HandleAnimations()
        {
            animator.SetBool("isRunning", currentState == State.Chase);
            animator.SetBool("isAttacking", currentState == State.Attack);
        }
        
        public void TakeDamage(float damageAmount)
        {
            if (currentState == State.Dead) return;
            
            currentHealth -= damageAmount;
            Debug.Log("Enemy took damage. Current health: " + currentHealth);
            
            animator.SetTrigger("GetHit");
            
            if (damageFeedback != null)
            {
                damageFeedback.TriggerDamageFlash();
            }
            
            if (currentHealth <= 0)
            {
                currentState = State.Dead;
                Die();
            }
            else
            {
                currentState = State.Hit; 
                animator.SetTrigger("GetHit");
            }
        }

        private void Die()
        {
            animator.SetTrigger("Die");
            DisableEnemy();
        }

        private void DisableEnemy()
        {
            // Disable components
            GetComponent<Collider>().enabled = false;
            navMeshAgent.enabled = false;
            // ... other components if needed
        }
        
        // This method will be called by an animation event at the end of the death animation
        public void OnDeathAnimationComplete()
        {
            Destroy(gameObject);
        }
        
        public void OnHitAnimationComplete()
        {
            /* Check what the character should be doing after recovering from the hit. For instance, if the player is within
            attack range, the enemy should transition to the Attack state. Otherwise, it might return to Idle or Chase. */

            var distanceToPlayer = Vector3.Distance(transform.position, player.position);
            if (distanceToPlayer <= attackRange)
            {
                currentState = State.Attack; 
            }
            else if (distanceToPlayer <= detectionRange)
            {
                currentState = State.Chase; 
            }
            else
            {
                currentState = State.Idle; 
            }
            
        }
    }
}
