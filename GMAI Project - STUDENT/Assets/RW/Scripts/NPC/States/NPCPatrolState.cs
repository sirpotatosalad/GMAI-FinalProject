using System.CodeDom;
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
        // populate waypoint array with NPC waypoints from the waypoint manager instance
        // can be done here as constructor is responsible for creating the initial state of the PatrolState object
        // i.e. akin to the Start() method
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

        // transition to Investigate state if NPC detects the player at suspiciousRange
        // essentially mimicks the NPC noticing some sort of presence, but still not being sure if it was the player
        // thus it will go to the player's last seen position and search around - discussed further in the Investigate state itself
        if (npc.DetectionCone(npc.suspiciousRange))
        {
            targetLocation = null;
            stateMachine.ChangeState(npc.investigate);
            Debug.Log("NPC - Suspicious of player presence");
        }

        // transition to Chase state if NPC detects the player at alertRange
        // i.e. the NPC has confirmed "seeing" the player character and intends to chase and attack                  
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

        // set currentWaypoint as destination if not already done so
        if (npc.agent.destination != targetLocation.position)
        {
            npc.agent.SetDestination(targetLocation.position);
        }

        // reset the targetLocation to null when reaching waypoint to allow the calculation of the next.
        if (!npc.agent.pathPending && npc.agent.remainingDistance < npc.agent.stoppingDistance)
        {
            targetLocation = null;
        }
    }

}
