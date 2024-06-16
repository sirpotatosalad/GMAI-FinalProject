using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Panda;
using UnityEngine.AI;
using System.Security.Permissions;
using RayWenderlich.Unity.StatePatternInUnity;

public class MonsterController : MonoBehaviour, IDamageable
{

    public Collider hitBox;

    public float detectionRange = 10f;
    public float detectionConeAngle = 60f;
    public float fleeDistance = 15f;
    public float rotationSpeed = 20f;

    public float playerStoppingDistance = 3.0f;
    public float pointStoppingDistance = 2.0f;

    public float chaseSpeed = 9.0f;
    public float patrolSpeed = 4.0f;

    public int maxHealth = 3;
    private int currentHealth;

    private Animator anim;
    private int walkParam = Animator.StringToHash("Walk Forward");
    private int runParam = Animator.StringToHash("Run Forward");
    private int dieParam = Animator.StringToHash("Die");
    private int takeDamageParam = Animator.StringToHash("Take Damage");


    private NavMeshAgent agent;

    [Task]
    public bool IsDead {  get; private set; }
    [Task]
    public bool IsMonsterInTerritory { get; private set; }

    // checks current health and sets IsLow bool based on creature health
    // used for FleePlayer task in MonsterTasks
    [Task]
    public bool IsLow()
    {
        return currentHealth <= 1;
    }
    // Start is called before the first frame update
    void Start()
    {
        IsDead = false;
        IsMonsterInTerritory = true;
        anim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();

        agent.speed = patrolSpeed;

        currentHealth = maxHealth;
    }

    // methods taken and copied from provided Character script in project
    // method names and functions are pretty self-explanatory, e.g. setting animator triggers and booleans
    public void SetAnimationBool(int param, bool value)
    {
        anim.SetBool(param, value);
    }

    public void TriggerAnimation(int param)
    {
        anim.SetTrigger(param);
    }

    [Task]
    public void ActivateHitBox()
    {
        hitBox.enabled = true;
        Task.current.Succeed();
    }

    [Task]
    public void DeactivateHitBox()
    {
        hitBox.enabled = false;
        Task.current.Succeed();
    }

    // TakeDamage method for damageable interface
    public void TakeDamage(int damage)
    {
        TriggerAnimation(takeDamageParam);
        currentHealth -= damage;
    }

    // action to make creature "die" once health reaches 
    [Task]
    public void Die()
    {
        TriggerAnimation(dieParam);
        Task.current.Succeed();
    }

    // Update is called once per frame
    void Update()
    {
        // check current creature health, setting IsDead to true if bot has reached 0 hp
        if (currentHealth <= 0 && !IsDead)
        {
            agent.isStopped = true;
            IsDead = true;
        }

        // sets provided creature's animator bool parameters based on agent's current speed
        // i.e. sets running/walking parameter bools when bot is patrolling/chasing the player
        if (agent.velocity.magnitude > 0)
        {
            if (agent.speed == chaseSpeed)
            {
                SetAnimationBool(runParam, true);
                SetAnimationBool(walkParam, false);
            }
            else if (agent.speed == patrolSpeed)
            {
                SetAnimationBool(runParam, false);
                SetAnimationBool(walkParam, true);
            }
        }
        else
        {
            SetAnimationBool(runParam, false);
            SetAnimationBool(walkParam, false);
        }
        
    }

    // check for bot collision within its territory,
    // setting IsMonsterInTerritory bool to its appropriate values as well
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("MonsterTerritory"))
        {
            Debug.Log("Monster in territory");
            IsMonsterInTerritory = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("Monster left territory");
        IsMonsterInTerritory = false;
    }
}
