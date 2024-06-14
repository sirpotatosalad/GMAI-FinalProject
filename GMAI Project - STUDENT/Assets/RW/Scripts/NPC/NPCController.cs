using JetBrains.Annotations;
using Panda.Examples.PlayTag;
using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.AI;



public class NPCController : MonoBehaviour, IDamageable
{
    public int maxHealth = 5;
    public int currentHealth;

    public float walkSpeed = 3.5f;
    public float runSpeed = 8f;

    public float giveUpTimer = 15f;
    public float suspiciousRange = 15f;
    public float alertRange = 7f;
    public float detectionConeAngle = 150f;
    public float attackRange = 1.75f;

    public Transform playerLastLocation;

    public NavMeshAgent agent;
    public Transform player;
    public Animator anim;

    public NPCStateMachine stateMachine;
    public NPCPatrolState patrol;
    public NPCInvestigateState investigate;
    public NPCChaseState chase;
    public NPCAttackState attack;


    private int horizonalMoveParam = Animator.StringToHash("H_Speed");
    private int verticalMoveParam = Animator.StringToHash("V_Speed");
    public int blockParam = Animator.StringToHash("Block");
    public int sprintParam = Animator.StringToHash("Sprint");
    public int lookParam = Animator.StringToHash("Looking");
    public int attackParam = Animator.StringToHash("Attack");
    public int hitParam = Animator.StringToHash("TakeDamage");
    

    // Start is called before the first frame update
    void Start()
    {
        stateMachine = new NPCStateMachine();

        patrol = new NPCPatrolState(this, stateMachine);
        investigate = new NPCInvestigateState(this, stateMachine);
        chase = new NPCChaseState(this, stateMachine);
        attack = new NPCAttackState(this, stateMachine);

        stateMachine.Initialize(patrol);

        currentHealth = maxHealth;
    }

    // making use of the IDamageable interface
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            // die
        }
    }

    public bool DetectionCone(float detectionRange)
    {
        float distance = Vector3.Distance(transform.position, player.position);
        if (distance > detectionRange)
        {
            return false;
        }

        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);

        if (angleToPlayer < detectionConeAngle / 2)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, directionToPlayer, out hit))
            {
                if (hit.transform == player)
                {
                    playerLastLocation = player.transform;
                    return true;
                }
                return false;
            }

        }
        return false;
    }

    public void SetAnimationBool(int param, bool value)
    {
        anim.SetBool(param, value);
    }

    public void TriggerAnimation(int param)
    {
        anim.SetTrigger(param);
    }

    public void LookAtPlayer()
    {
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(directionToPlayer);
        transform.rotation = lookRotation;
    }

    public bool IsPlayerInRange(float range)
    {
        float distance = Vector3.Distance(transform.position, player.position);
        return distance <= range;
    }

    // Update is called once per frame
    void Update()
    {
        stateMachine.CurrentState.HandleInput();
        stateMachine.CurrentState.LogicUpdate();
        UpdateAnimatorFloats();
    }

    private void FixedUpdate()
    {
       stateMachine.CurrentState.PhysicsUpdate();
    }

    private void UpdateAnimatorFloats()
    {
        Vector3 velocity = agent.velocity;

        Vector3 localVelocity = transform.InverseTransformDirection(velocity);

        anim.SetFloat(horizonalMoveParam, localVelocity.x);
        anim.SetFloat(verticalMoveParam, localVelocity.z);
    }


}


