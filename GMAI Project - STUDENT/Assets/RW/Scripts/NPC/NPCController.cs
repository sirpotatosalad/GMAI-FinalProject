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
    


    // making use of the IDamageable interface to handle damage taken from player
    public void TakeDamage(int damage)
    {
        float probability = Random.Range(0f,1f);
        Debug.Log("NPC - Hit taken");

        // only handle damage if NPC is not blocking
        if (stateMachine.CurrentState != block)
        {
            // when NPC is not alerted to the player and attacked from behind
            if (stateMachine.CurrentState != chase)
            {
                currentHealth -= damage;
                stateMachine.ChangeState(chase);
            }
            
            // chance for NPC to block damage taken when being attacked by the player and is alerted
            if (probability < blockChance && (stateMachine.CurrentState == attack || stateMachine.CurrentState == chase))
            {
                stateMachine.ChangeState(block);
                return;
            }

            // chance for npc to get stunned when taking damage
            // mimicks taking a "critical hit" from the player
            if (probability < stunChance)
            {
                currentHealth -= damage;
                TriggerAnimation(hitParam);
                stateMachine.ChangeState(stunned);
                return;
            }

            // handle getting hit if players attack hits succesfully
            TriggerAnimation(hitParam);
            currentHealth -= damage;

            // kill NPC and switch to dead state if health is 0
            if (currentHealth <= 0)
            {
                stateMachine.ChangeState(dead);
            }
        }

    }

    // same method of detection as seen in MonsterTasks
    // renamed for easier use and identification across the NPC states  
    public bool DetectionCone(float detectionRange)
    {
        // check if player is in range to be detected
        float distance = Vector3.Distance(transform.position, player.position);
        if (distance > detectionRange)
        {
            return false;
        }

        // check the angle value from the NPC's forward vector to the player's position
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);

        // check if angle from NPC forward vector to player is within the NPC's detection cone
        // detectionConeAngle / 2, as the angle is taken from the NPC's forward vector
        // i.e. if cone is 150 deg wide, then NPC can only see the player 75 deg away from its forward vector
        if (angleToPlayer < detectionConeAngle / 2)
        {
            // make a raycast to check for any obstacles between player and NPC
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

    // same methods copied over from the provided Character script

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

    public bool IsAnimatorPlaying(int animLayer, string stateName)
    {
        //checks if normalized time < 1, i.e. when an animation clip has completed playing
        return anim.GetCurrentAnimatorStateInfo(animLayer).normalizedTime < 1.0f
            // check for the specified state via its name
            && anim.GetCurrentAnimatorStateInfo(animLayer).IsName(stateName);
    }


     // same methods copied over from MonsterController script
    public void LookAtPlayer()
    {
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(directionToPlayer);
        transform.rotation = lookRotation;
    }

    // since the NPC FSM uses the same State and StateMachine class structures as the player character
    // the monobehaviour callbacks are also similar in logic to the provided Character script
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

    void Update()
    {
        stateMachine.CurrentState.HandleInput();
        stateMachine.CurrentState.LogicUpdate();

        // update float parameters in anim controller during runtime
        UpdateAnimatorFloats();
    }

    private void FixedUpdate()
    {
       stateMachine.CurrentState.PhysicsUpdate();
    }

    // obtain velocity from NPC's NavMeshAgent component to assign to the float parameters in the NPC animator controller
    private void UpdateAnimatorFloats()
    {
        Vector3 velocity = agent.velocity;

        //  implemented with some help from ChatGPT and looking through Unity documentation
        // InverseTransformDirection essentially converts a direction vector from world space to local space relative to the GameObject's transform
        // by converting the vector to local space, it is possible to obtain the values relative to the GameObject's orientation and scale
        // in this case, we can obtain the forward and horizontal values relative to the objects movement, similar to obtain the horizontal input from the player
        Vector3 localVelocity = transform.InverseTransformDirection(velocity);

        //set respective localVelocity values to their animator float parameters
        anim.SetFloat(horizonalMoveParam, localVelocity.x);
        anim.SetFloat(verticalMoveParam, localVelocity.z);
    }


}


