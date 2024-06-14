using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCAttackState : NPCState
{
    public NPCAttackState(NPCController npc, NPCStateMachine stateMachine) : base(npc, stateMachine) { }

    public override void Enter()
    {
        base.Enter();
        Debug.Log("NPC - Entered state: ATTACK");
        npc.agent.isStopped = true;
        AttackPlayer();
    }

    private void AttackPlayer()
    {
        Debug.Log("Attacking player");
        npc.LookAtPlayer();
        npc.TriggerAnimation(npc.attackParam);
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if(!npc.IsPlayerInRange(npc.attackRange))
        {
            stateMachine.ChangeState(npc.chase);
        }

        AttackPlayer();
    }

    public override void Exit()
    {
        base.Exit();
        npc.agent.isStopped = false;
    }

}
