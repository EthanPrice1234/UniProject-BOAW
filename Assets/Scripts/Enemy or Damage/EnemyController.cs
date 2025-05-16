using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    [Header("Enemy Engagement Parameters")]
    public float lookRadius = 1000f;
    public float engageDistance = 10f;
    private float circlingRadius = 7f;
    public bool isCombatEngaged = false;

    [Header("Attacking Parameters")]
    public bool isAttacking = false;
    public float attackDelay = 2f;

    [Header("Speed Parameters")]
    public float normalSpeed = 2.5f;
    public float boostedSpeed = 6f;
    public float normalAnimSpeed = 1f;
    public float boostedAnimSpeed = 1.5f;
    public float speedBoostDistance = 15f; 

    // Components
    Transform target;
    GameObject player;
    Animator animator;
    EnemyStats enemyStats;
    NavMeshAgent agent;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        enemyStats = GetComponent<EnemyStats>();
        player = GameObject.FindGameObjectWithTag("Player");
        target = player.transform;
        
        agent.speed = normalSpeed;
        animator.speed = normalAnimSpeed;
    }

    void Update()
    {
        // Attack delay
        attackDelay -= Time.deltaTime;

        // Distance to player
        float distance = Vector3.Distance(target.position, transform.position);

        // Disengage if outside engagement distance
        if (distance > engageDistance && isCombatEngaged)
        {
            isCombatEngaged = false;
            if (CombatController.Instance != null)
                CombatController.Instance.Disengage(this);
        }
        
        // Check if player is within follow range of the enemy
        if (distance <= lookRadius && !enemyStats.isTakingDamage)
        {
            // Check if enemy is far enough to need a speed boost to catch up
            if (distance > speedBoostDistance)
            {
                agent.speed = boostedSpeed;
                animator.speed = boostedAnimSpeed;
            }
            else
            {
                agent.speed = normalSpeed;
                animator.speed = normalAnimSpeed;
            }
            
            // If the enemy is not comabt engaged, try to engage, if not wait outside the circlingRadius
            if (!isCombatEngaged)
            {
                // Try to engage
                if (distance < engageDistance && !isCombatEngaged && CombatController.Instance.TryEngage(this))
                {   
                    Debug.Log($"[EnemyController] {gameObject.name} successfully engaged with CombatController {CombatController.Instance.gameObject.name}");
                    isCombatEngaged = true;
                }
                else
                {
                    // If engagement fails, move to the circling radius
                    if (isCombatEngaged)
                    {
                        Debug.Log($"[EnemyController] {gameObject.name} disengaged due to distance or failed engagement attempt");
                    }
                    isCombatEngaged = false;
                    Vector3 circlePos = GetCirclePosition();
                    float distToCircle = Vector3.Distance(transform.position, circlePos);

                    // Check if the enemy is close enough to the circle position
                    if (distToCircle > 1f)
                    {
                        // Move to the circle position
                        agent.isStopped = false;
                        agent.SetDestination(circlePos);
                        animator.SetBool("isMoving", true);
                    }
                    else
                    {
                        // Stop the enemy from moving
                        agent.isStopped = true;
                        agent.ResetPath();
                        animator.SetBool("isMoving", false);
                    }
                    FaceTarget();
                    return;
                }
            }
            // Enemy is engaged nowmove to attack the player
            else
            {
                // If the enemy is not attacking, move to the player
                if (!isAttacking)
                {
                    agent.isStopped = false;
                    agent.SetDestination(target.position);
                    animator.SetBool("isMoving", true);
                }

                // If the enemy is close enough to the player, stop moving and try to attack
                if (distance <= agent.stoppingDistance)
                {
                    FaceTarget();
                    agent.isStopped = true;
                    agent.ResetPath();
                    animator.SetBool("isMoving", false);

                    // Attempt attack if not already attacking
                    if (!isAttacking && attackDelay <= 0)
                    {
                        animator.SetBool("isAttacking", true);
                        isAttacking = true;
                        attackDelay = 5f; 
                    }
                }
            }
        }
    }

    void FaceTarget()
    {
        Vector3 direction = (target.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }

    Vector3 GetCirclePosition()
    {
        Vector3 dir = (transform.position - target.position).normalized;
        return target.position + dir * circlingRadius;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, lookRadius);
        
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, speedBoostDistance);
    }

    public void ResetAttacking()
    {
        isAttacking = false;
        animator.SetBool("isAttacking", false);
    }

    void OnDisable()
    {
        if (isCombatEngaged && CombatController.Instance != null)
            CombatController.Instance.Disengage(this);
    }
}
