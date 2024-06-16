using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;

public class NPCInvestigateState : NPCState
{
    private float giveUpTimer;
    private bool isLookingAround = false;

    private Vector3[] lookDirections;
    private Quaternion targetRotation;
    private int directionIndex = 0;
    private float lookDuration = 2f;
    private float lookTimer;

    public NPCInvestigateState(NPCController npc, NPCStateMachine stateMachine) : base(npc, stateMachine) 
    {
        giveUpTimer = npc.giveUpTimer;

        // initialise lookDirections array with the 4 directions the NPC will cycle through when investigating an area
        // i.e., the forward, backward (-forward), right and left (-right) vectors of the NPC's transform
        lookDirections = new Vector3[] 
        {
            npc.transform.forward,
            npc.transform.right,
            -npc.transform.forward,
            -npc.transform.right
        };
    }

    public override void Enter()
    {
        base.Enter();
        Debug.Log("NPC - Entered state: INVESTIGATE");
        // set NPC to "looking" animation
        npc.SetAnimationBool(npc.lookParam, true);
        // set the NPC to move toward the player's last seen location
        npc.agent.SetDestination(npc.playerLastLocation.position);
        targetRotation = npc.transform.rotation;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        // chase the player if they are spotted within the NPC's alertRange
        if (npc.DetectionCone(npc.alertRange))
        {
            Debug.Log("NPC - Player found");
            stateMachine.ChangeState(npc.chase);
        }
        // update the last seen location if player is still within NPC's suspiciousRange
        else if (npc.DetectionCone(npc.suspiciousRange))
        {
            npc.agent.SetDestination(npc.playerLastLocation.position);
            isLookingAround = false;
        }

        // move NPC to player's last seen location before looking around
        if (!isLookingAround)
        {
            GoToLastSeen();
        }
        // once NPC has moved to the player's last seen position, look around and investigate for player presence at that point.
        else
        {
            Investigate();
        }
    }

    // check if NPC is still pathing to player's last seen position
    // if so, stops the navmesh agent and sets the NPC to look around for the player
    private void GoToLastSeen()
    {
        if (!npc.agent.pathPending && npc.agent.remainingDistance < npc.agent.stoppingDistance)
        {
            isLookingAround = true;
            npc.agent.isStopped = true;
            
        }
    }

    private void Investigate()
    {
        
        lookTimer -= Time.deltaTime;
        giveUpTimer -= Time.deltaTime;

        // NPC stops investigating area / looking for player and gives up after giveUpTimer depletes
        // transitions back to Patrol state to follow its patrol route
        if (giveUpTimer <= 0 )
        {
            giveUpTimer = npc.giveUpTimer;
            Debug.Log("NPC - Did not find anything");
            stateMachine.ChangeState(npc.patrol);
        }

        // rotate the NPC about its position
        // mimicks the NPC searching for signs of the player at its current position
        npc.transform.rotation = Quaternion.Lerp(npc.transform.rotation, targetRotation, Time.deltaTime * 2f);

        // after the NPC has been looking at a certain direction for lookTimer, rotate to the next lookDirection
        if (lookTimer <= 0f)
        {
            lookTimer = lookDuration;
            // once the directionIndex pointer reaches the last index of lookDirections, makes use of modulo operator to set back to first index 
            directionIndex = (directionIndex + 1) % lookDirections.Length;
            // rotate player to new direction
            targetRotation = Quaternion.LookRotation(lookDirections[directionIndex]);
        }
    }

    public override void Exit()
    {
        base.Exit();
        
        isLookingAround = false;
        // ensure NPC's agent can continue moving and stop the NPC from playing its "looking" animation
        npc.agent.isStopped = false;
        npc.SetAnimationBool(npc.lookParam, false);
    }
}
