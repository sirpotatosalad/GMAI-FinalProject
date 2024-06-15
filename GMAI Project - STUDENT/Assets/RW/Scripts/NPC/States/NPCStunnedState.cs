using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCStunnedState : NPCState
{
    private float stunTimer;
    public NPCStunnedState(NPCController npc, NPCStateMachine stateMachine) : base(npc, stateMachine) {}

    public override void Enter()
    {
        base.Enter();
        Debug.Log("NPC - Stunned");
        npc.SetAnimationBool(npc.stunParam, true);
        stunTimer = 0;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        stunTimer += Time.deltaTime;

        if (stunTimer >= npc.stunDuration)
        {
            stateMachine.ChangeState(npc.patrol);
        }
    }

    public override void Exit()
    {
        base.Exit();
        npc.SetAnimationBool(npc.stunParam, false);
        stunTimer = 0;
    }
}
