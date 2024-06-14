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
        npc.agent.speed = npc.runSpeed;
        npc.SetAnimationBool(npc.sprintParam, true);
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        ChasePlayer();
    }

    private void ChasePlayer()
    {
        if (npc.player != null)
        {
            npc.agent.SetDestination(npc.player.position);
        }

        if (!npc.IsPlayerInRange(npc.suspiciousRange))
        {
            Debug.Log("NPC - Lost sight of player");
            stateMachine.ChangeState(npc.investigate);
        }

        if (npc.IsPlayerInRange(npc.attackRange))
        {
            Debug.Log("NPC - Player in attack range");
            stateMachine.ChangeState(npc.attack);
        }

    }

    public override void Exit()
    {
        base.Exit();
        npc.agent.speed = npc.walkSpeed;
        npc.SetAnimationBool(npc.sprintParam, false);
        npc.agent.ResetPath();
    }
}
