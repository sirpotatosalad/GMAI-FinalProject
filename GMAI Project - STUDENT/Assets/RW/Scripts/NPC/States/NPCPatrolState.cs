using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPCPatrolState : NPCState
{


    private int currentWaypoint;
    private Transform targetLocation;
    private Transform[] patrolWaypoints;

    public NPCPatrolState(NPCController npc, NPCStateMachine stateMachine) : base(npc, stateMachine) 
    {
        patrolWaypoints = WaypointManager.instance.NPCWaypoints;
    }

    public override void Enter()
    {
        base.Enter();
        Debug.Log("NPC - Entered state: PATROL");
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        PatrolArea();

        if (npc.DetectionCone(npc.suspiciousRange))
        {
            //transition to investigate
            targetLocation = null;
            stateMachine.ChangeState(npc.investigate);
            Debug.Log("NPC - Suspicious of player presence");
        }

        if (npc.DetectionCone(npc.alertRange))
        {
            targetLocation = null;
            stateMachine.ChangeState(npc.chase);
        }

        

    }

    private void PatrolArea()
    {
        if (targetLocation == null)
        {
            // set the agent's destination to the current waypoint index
            targetLocation = patrolWaypoints[currentWaypoint];
            Debug.Log(currentWaypoint);

            // once the currentWaypoint pointer reaches the last index of Waypoint, makes use of modulo operator to set back to first index
            // i.e. operator will return the remainder of the currentWaypoint calculation, effectively returning a value between 0 and the length of the Waypoint list.
            currentWaypoint = (currentWaypoint + 1) % patrolWaypoints.Length;
        }

        npc.agent.stoppingDistance = 1.5f;


        if (npc.agent.destination != targetLocation.position)
        {
            npc.agent.SetDestination(targetLocation.position);
        }

        // for here, the bot will continually investigate the area until it doesn't find any clues within a set amount of time
        // once it reaches that set amount of time, it "gives up" on trying to search more - believing to have already seen everything
        if (!npc.agent.pathPending && npc.agent.remainingDistance < npc.agent.stoppingDistance)
        {
            targetLocation = null;
        }
    }

}
