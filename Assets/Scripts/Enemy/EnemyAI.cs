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
            Attack
        }

        private State currentState = State.Idle;

        [SerializeField] private Transform player;
        [SerializeField] private float detectionRange = 5f;
        [SerializeField] private float attackRange = 1f;

        private Animator animator;
        private NavMeshAgent navMeshAgent;
    
        [SerializeField] private float attackDamage = 10f;
        [SerializeField] private float attackDelay = 1f; // Time delay between successive attacks
        private float lastAttackTime;
        
        [SerializeField] private float maxHealth = 100f;
        private float currentHealth;

        private void Awake()
        {
            animator = GetComponent<Animator>();
            navMeshAgent = GetComponent<NavMeshAgent>();
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
            currentHealth -= damageAmount;
            Debug.Log("Enemy took damage. Current health: " + currentHealth);

            if (currentHealth <= 0)
            {
                Die();
            }
        }

        private void Die()
        {
            Destroy(gameObject);
        }
    }
}
