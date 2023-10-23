using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

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

    private void Awake()
    {
        animator = GetComponent<Animator>();
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

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
                if (distanceToPlayer > attackRange)
                {
                    currentState = State.Chase;
                }
                break;
        }
        HandleAnimations();
        
        // Debug.Log("Distance to Player: " + distanceToPlayer);
        Debug.Log("Current State: " + currentState.ToString());
    }
    
    private void HandleAnimations()
    {
        animator.SetBool("isRunning", currentState == State.Chase);
        animator.SetBool("isAttacking", currentState == State.Attack);
    }
}
