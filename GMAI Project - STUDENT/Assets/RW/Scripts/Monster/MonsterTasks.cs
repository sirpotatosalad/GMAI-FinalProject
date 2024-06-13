using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Panda;

public class MonsterTasks : MonoBehaviour
{

    private int attack1Param = Animator.StringToHash("Attack 01");

    private MonsterController monsterController;
    private Animator anim;
    private NavMeshAgent agent;

    [SerializeField]
    private Transform player;


    private Transform targetLocation;
    private int currentWaypoint;
    private int previousWaypoint;
    private Transform[] PatrolPoints;
    private bool isReturningToPatrol = false;

    // Start is called before the first frame update
    void Start()
    {
        monsterController = GetComponent<MonsterController>();
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();

        PatrolPoints = WaypointManager.instance.Waypoints;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [Task]
    public void GoToPlayer()
    {


        agent.speed = monsterController.patrolSpeed;

        targetLocation = player;

        agent.stoppingDistance = monsterController.playerStoppingDistance;

        // sets the navmesh agent's destination to the player's transform position
        if (agent.destination != targetLocation.transform.position)
        {
            agent.SetDestination(targetLocation.position);
        }

        // succeeds the task after reaching the specified stopping distance away from the player
        if (!agent.pathPending && agent.remainingDistance < agent.stoppingDistance)
        {
            Task.current.Succeed();
            targetLocation = null;
        }
    }

    [Task]
    public void ChasePlayer()
    {
        agent.speed = monsterController.chaseSpeed;

        targetLocation = player;

        if (!IsPlayerVisible())
        {
            agent.ResetPath();
            Task.current.Fail();
            return;
        }

        // sets the navmesh agent's destination to the player's transform position
        if (agent.destination != targetLocation.transform.position)
        {
            agent.SetDestination(targetLocation.position);
        }

        // succeeds the task after reaching the specified stopping distance away from the player
        if (!agent.pathPending && agent.remainingDistance < agent.stoppingDistance)
        {
            Task.current.Succeed();
            targetLocation = null;
        }
    }

    [Task]
    public void FleePlayer()
    {
        isReturningToPatrol = true;

        Vector3 fleeDir = (transform.position - player.position).normalized;
        Vector3 fleePos = transform.position + fleeDir * monsterController.fleeDistance;

        NavMeshHit hit;

        if(NavMesh.SamplePosition(fleePos, out hit, monsterController.fleeDistance, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
            Task.current.Succeed();
        }
        else
        {
            Task.current.Fail();
        }
    }

    [Task]
    public void Patrol()
    {


        if (targetLocation == null)
        {

            agent.speed = monsterController.patrolSpeed;

            // case for when bot's pathing deviates due to "discovering" a clue
            // instead of returning to the previous waypoint, it will go to the nearest one, and continue from there
            if (isReturningToPatrol)
            {
                Transform closestWaypoint = WaypointManager.instance.GetClosestWaypoint(transform.position);

                // find the index of the closest waypoint, set it to currentWaypoint index
                for (int i = 0; i < PatrolPoints.Length; i++)
                {
                    if (PatrolPoints[i] == closestWaypoint)
                    {
                        currentWaypoint = i;
                        break;
                    }
                }

                isReturningToPatrol = false;
            }


            // set the agent's destination to the current waypoint index
            targetLocation = PatrolPoints[currentWaypoint];
            Debug.Log(currentWaypoint);

            // once the currentWaypoint pointer reaches the last index of Waypoint, makes use of modulo operator to set back to first index
            // i.e. operator will return the remainder of the currentWaypoint calculation, effectively returning a value between 0 and the length of the Waypoint list.
            currentWaypoint = (currentWaypoint + 1) % PatrolPoints.Length;

        }


        // as mentioned above, the following lines are similar to that of MoveToTaggedObject()

        agent.stoppingDistance = monsterController.pointStoppingDistance;


        if (agent.destination != targetLocation.transform.position)
        {
            agent.SetDestination(targetLocation.position);
        }

        // for here, the bot will continually investigate the area until it doesn't find any clues within a set amount of time
        // once it reaches that set amount of time, it "gives up" on trying to search more - believing to have already seen everything
        if (!agent.pathPending && agent.remainingDistance < agent.stoppingDistance)
        {
            Task.current.Succeed();
            targetLocation = null;
        }
    }

    [Task]
    public void Attack()
    {
        monsterController.TriggerAnimation(attack1Param);
        Task.current.Succeed();
    }

    [Task]
    public void TakeDamage()
    {

    }

    [Task]
    public void Idle()
    {
        Debug.Log("Monster is currently idling");
        Task.current.Succeed();
    }

    [Task]
    bool IsPlayerInAttackRange()
    {
        float distance = Vector3.Distance(transform.position, player.position);

        return distance < monsterController.playerStoppingDistance;
    }

    [Task]
    bool IsPlayerVisible()
    {
        float distance = Vector3.Distance(transform.position, player.position);

        return distance < monsterController.detectionRange;
    }


}
