using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCChaseState : NPCState
{
    public NPCChaseState(NPCController npc, NPCStateMachine stateMachine) : base(npc, stateMachine)  { }

    public override void Enter()
    {
        base.Enter();
        Debug.Log("NPC - Entered state: CHASE");
        // set NPC's speed to runSpeed and set their animation to sprinting
        npc.agent.speed = npc.runSpeed;
        npc.SetAnimationBool(npc.sprintParam, true);
    }

    // as discussed below, call ChasePlayer() here to continuously seek and chase after the player
    public override void LogicUpdate()
    {
        base.LogicUpdate();
        ChasePlayer();
    }

    private void ChasePlayer()
    {
        // set agent's destination to the player
        // since this is called in LogicUpdate(), it will continuously update and seek the player
        if (npc.player != null)
        {
            npc.agent.SetDestination(npc.player.position);
        }

        // transition to Investigate state and path to player's last known position if player leaves NPC's suspiciousRange
        if (!npc.DetectionCone(npc.suspiciousRange))
        {
            Debug.Log("NPC - Lost sight of player");
            stateMachine.ChangeState(npc.investigate);
        }

        // if NPC is close enough to the player, transition to Attack state and attack them
        if (npc.DetectionCone(npc.attackRange))
        {
            Debug.Log("NPC - Player in attack range");
            stateMachine.ChangeState(npc.attack);
        }

    }

    public override void Exit()
    {
        base.Exit();
        // change agent speed back to its original walking speed and set the sprint animation parameter accordingly
        npc.agent.speed = npc.walkSpeed;
        npc.SetAnimationBool(npc.sprintParam, false);
        npc.agent.ResetPath();
    }
}
