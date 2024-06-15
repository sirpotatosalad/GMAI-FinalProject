using JetBrains.Annotations;
using Panda.Examples.PlayTag;
using RayWenderlich.Unity.StatePatternInUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.AI;



public class NPCController : MonoBehaviour, IDamageable
{
    public int maxHealth = 5;
    private int currentHealth;

    public float walkSpeed = 3.5f;
    public float runSpeed = 8f;

    public float giveUpTimer = 15f;
    public float suspiciousRange = 15f;
    public float alertRange = 7f;
    public float detectionConeAngle = 150f;

    public float attackRange = 1.75f;
    public float attackDelay = 1.5f;
    public float blockChance = 0.3f;
    public float stunChance = 0.1f;
    public float stunDuration = 7f;

    public Transform playerLastLocation;

    public NavMeshAgent agent;
    public Transform player;
    public Animator anim;
    [SerializeField]
    private Collider npcHitBox;

    public NPCStateMachine stateMachine;
    public NPCPatrolState patrol;
    public NPCInvestigateState investigate;
    public NPCChaseState chase;
    public NPCAttackState attack;
    public NPCStunnedState stunned;
    public NPCBlockState block;
    public NPCDeadState dead;


    private int horizonalMoveParam = Animator.StringToHash("H_Speed");
    private int verticalMoveParam = Animator.StringToHash("V_Speed");
    public int blockParam = Animator.StringToHash("Block");
    public int sprintParam = Animator.StringToHash("Sprint");
    public int lookParam = Animator.StringToHash("Looking");
    public int stunParam = Animator.StringToHash("Stunned");
    public int deadParam = Animator.StringToHash("Dead");
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
        stunned = new NPCStunnedState(this, stateMachine);
        block = new NPCBlockState(this, stateMachine);
        dead = new NPCDeadState(this, stateMachine);

        stateMachine.Initialize(patrol);

        currentHealth = maxHealth;
    }

    // making use of the IDamageable interface
    public void TakeDamage(int damage)
    {
        float probability = Random.Range(0f,1f);
        Debug.Log("NPC - Hit taken");

        if (stateMachine.CurrentState != block)
        {
            if (currentHealth <= 0)
            {
                stateMachine.ChangeState(dead);
                return;
            }

            if (stateMachine.CurrentState != attack && stateMachine.CurrentState != stunned)
            {
                stateMachine.ChangeState(chase);
            }

            if (probability < blockChance && stateMachine.CurrentState == attack)
            {
                stateMachine.ChangeState(block);
                return;
            }

            if (probability < stunChance)
            {
                currentHealth -= damage;
                TriggerAnimation(hitParam);
                stateMachine.ChangeState(stunned);
                return;
            }

            TriggerAnimation(hitParam);
            currentHealth -= damage;
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

    public void ActivateHitBox()
    {
        npcHitBox.enabled = true;
    }

    public void DeactivateHitBox()
    {
        npcHitBox.enabled = false;
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

    public bool IsAnimatorPlaying(int animLayer, string stateName)
    {
        //checks if normalized time < 1, i.e. when an animation clip has completed playing
        return anim.GetCurrentAnimatorStateInfo(animLayer).normalizedTime < 1.0f
            // check for the specified state via its name
            && anim.GetCurrentAnimatorStateInfo(animLayer).IsName(stateName);
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


