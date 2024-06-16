using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Panda;
using RayWenderlich.Unity.StatePatternInUnity;
using UnityEngine.TextCore.Text;

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
    private Transform[] PatrolPoints;
    private bool isReturningToPatrol = false;
    [Task]
    private bool IsChasing = false;


    [SerializeField]
    private Transform patrolAreaCenter;
    public float patrolAreaSize = 20f;

    // Start is called before the first frame update
    void Start()
    {
        monsterController = GetComponent<MonsterController>();
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();

        targetLocation = null;
        PatrolPoints = WaypointManager.instance.MonsterWaypoints;
    }

    // action to move creature to the player's position
    [Task]
    public void GoToPlayer()
    {
        // stops moving toward the player if not visible by creature
        if (!IsPlayerVisible())
        {
            targetLocation = null;
            isReturningToPatrol = true;
            agent.ResetPath();
            Task.current.Fail();
            return;
        }

        // bot is "passive" in this state, only walking up toward the player
        agent.speed = monsterController.patrolSpeed;

        targetLocation = player;

        agent.stoppingDistance = monsterController.playerStoppingDistance;

        // sets the navmesh agent's destination to the player's transform position, essentially "tracking" the player while the tree ticks
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

    // similar to ObservePlayer, but instead will make bot run after the player with intent to attack
    [Task]
    public void ChasePlayer()
    {
        IsChasing = true;

        agent.speed = monsterController.chaseSpeed;

        targetLocation = player;

        if (!IsPlayerVisible())
        {
            targetLocation = null;
            isReturningToPatrol = true;
            IsChasing = false;
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
            targetLocation = null;
            IsChasing = false;
            Task.current.Succeed();
        }
    }
    
    // "observes" player if creature detects them walking around outside its territory by walking toward and looking at them
    // will continue to observe the player until it loses sight of them
    [Task]
    public void ObservePlayer()
    {
        if (!IsPlayerVisible())
        {
            Task.current.Fail();
            return;
        }

        LookAtPlayer();
        Task.current.Succeed();
    }
    
    // when the creature has low health, it will run away from the player
    [Task]
    public void FleePlayer()
    {
        isReturningToPatrol = true;

        // find the direction and possible flee position away from the player
        Vector3 fleeDir = (transform.position - player.position).normalized;
        Vector3 fleePos = transform.position + fleeDir * monsterController.fleeDistance;

        // if creature is able to run toward that position, succeed task and "flee" toward it
        // makes use of NavMesh plugin's provided SamplePosition, essentially checking if the provided position is acessible by the agent
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

        agent.speed = monsterController.patrolSpeed;

        if (isReturningToPatrol)
        {
            agent.isStopped = true;
            targetLocation = null;
        }

        if (targetLocation == null)
        {

            agent.isStopped = false;

            // case for when bot's pathing deviates due to spotting the player
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



        agent.stoppingDistance = monsterController.pointStoppingDistance;


        if (agent.destination != targetLocation.position)
        {
            agent.SetDestination(targetLocation.position);
        }

        // succeed task once it reaches a patrol waypoint
        if (!agent.pathPending && agent.remainingDistance < agent.stoppingDistance)
        {
            Task.current.Succeed(); 
            targetLocation = null;
        }
    }

    // action for creature to attack the player once within attack range
    [Task]
    public void Attack()
    {
        LookAtPlayer();

        monsterController.TriggerAnimation(attack1Param);

        Task.current.Succeed();
    }

    [Task]
    public void Idle()
    {
        Debug.Log("Monster is currently idling");
        Task.current.Succeed();
    }

    // checks if bot is close enough to the player to be able to attack them
    // simply makes use of the playerStoppingDistance that is applied to the navmesh agent
    [Task]
    bool IsPlayerInAttackRange()
    {
        float distance = Vector3.Distance(transform.position, player.position);

        return distance < monsterController.playerStoppingDistance;
    }

    // method checks if bot can see player within its cone of view
    [Task]
    bool IsPlayerVisible()
    {
        // check if player is in range to be detected
        float distance = Vector3.Distance(transform.position, player.position);
        if (distance > monsterController.detectionRange)
        {
            return false;
        }

        // check the angle value from the creature's forward vector to the player's position
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);

        // check if angle from creature forward vector to player is within the creature's detection cone
        // detectionConeAngle / 2, as the angle is taken from the creature's forward vector
        // i.e. if cone is 150 deg wide, then creature can only see the player 75 deg away from its forward vector
        if (angleToPlayer < monsterController.detectionConeAngle / 2)
        {
            // make a raycast to check for any obstacles between player and creature
            RaycastHit hit;
            if (Physics.Raycast(transform.position, directionToPlayer, out hit))
            {
                if (hit.transform == player)
                {
                    return true;
                }
                return false;
            }
            
        }
        return false;
    }

    // gets IsInMonsterTerritory bool from player's Character script
    [Task]
    bool IsPlayerInTerritory()
    {
        return player.GetComponent<RayWenderlich.Unity.StatePatternInUnity.Character>().IsInMonsterTerritory;
    }

    // method is pretty self explanatory
    // essentially rotates the creature to face in the direction of the player
    private void LookAtPlayer()
    {
        // get vector direction to player
        // set a new lookRotation using Quaternion.LookRotation with vector direction
        // set the creature's new rotation vector
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(directionToPlayer);
        transform.rotation = lookRotation;
    }


    
}
