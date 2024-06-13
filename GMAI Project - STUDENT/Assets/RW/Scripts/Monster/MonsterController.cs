using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Panda;
using UnityEngine.AI;
using System.Security.Permissions;

public class MonsterController : MonoBehaviour
{

    public float detectionRange = 10f;
    public float fleeDistance = 15f;

    public float playerStoppingDistance = 3.0f;
    public float pointStoppingDistance = 2.0f;

    public float chaseSpeed = 9.0f;
    public float patrolSpeed = 4.0f;

    public int hitPoints = 3;

    private Animator anim;
    private int walkParam = Animator.StringToHash("Walk Forward");
    private int runParam = Animator.StringToHash("Run Forward");
    private int dieParam = Animator.StringToHash("Die");
    private int takeDamageParam = Animator.StringToHash("Take Damage");

    private bool hasDied = false;

    private NavMeshAgent agent;

    [Task]
    public bool IsDead {  get; private set; }

    [Task]
    public bool IsLow()
    {
        return hitPoints <= 1;
    }
    // Start is called before the first frame update
    void Start()
    {
        IsDead = false;
        anim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();

        agent.speed = patrolSpeed;
    }
    public void SetAnimationBool(int param, bool value)
    {
        anim.SetBool(param, value);
    }

    public void TriggerAnimation(int param)
    {
        anim.SetTrigger(param);
    }

    public void TakeDamage()
    {
        hitPoints--;
    }

    [Task]
    public void Die()
    {
        if (!hasDied)
        {
            TriggerAnimation(dieParam);
            hasDied = true;
        }
        
        Task.current.Succeed();
    }

    // Update is called once per frame
    void Update()
    {

        if (hitPoints <= 0 && !IsDead)
        {
            agent.isStopped = true;
            IsDead = true;
        }

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
}
