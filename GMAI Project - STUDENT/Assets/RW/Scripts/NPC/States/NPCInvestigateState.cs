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
        npc.SetAnimationBool(npc.lookParam, true);
        npc.agent.SetDestination(npc.playerLastLocation.position);
        targetRotation = npc.transform.rotation;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (npc.DetectionCone(npc.alertRange))
        {
            Debug.Log("NPC - Player found");
            stateMachine.ChangeState(npc.chase);
        }
        else if (npc.DetectionCone(npc.suspiciousRange))
        {
            npc.agent.SetDestination(npc.playerLastLocation.position);
        }

        if (!isLookingAround)
        {
            GoToLastSeen();
        }
        else
        {
            Investigate();
        }
    }

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

        if (giveUpTimer <= 0 )
        {
            giveUpTimer = npc.giveUpTimer;
            Debug.Log("NPC - Did not find anything");
            stateMachine.ChangeState(npc.patrol);
        }

        npc.transform.rotation = Quaternion.Lerp(npc.transform.rotation, targetRotation, Time.deltaTime * 2f);

        if (lookTimer <= 0f)
        {
            lookTimer = lookDuration;
            directionIndex = (directionIndex + 1) % lookDirections.Length;
            targetRotation = Quaternion.LookRotation(lookDirections[directionIndex]);
        }
    }

    public override void Exit()
    {
        base.Exit();
        
        isLookingAround = false;
        npc.agent.isStopped = false;
        npc.SetAnimationBool(npc.lookParam, false);
    }
}
