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
        // set NPC's animator to play stunned animation and stop the agent from moving
        npc.SetAnimationBool(npc.stunParam, true);
        npc.agent.isStopped = true;
        stunTimer = 0;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        stunTimer += Time.deltaTime;

        // transition back to Investigate state after being stunned
        // i.e. NPC will go to where the player was last seen before it was stunned to attempt to search for them.
        if (stunTimer >= npc.stunDuration)
        {
            stateMachine.ChangeState(npc.investigate);
        }
    }

    public override void Exit()
    {
        base.Exit();
        // revert NPC to its inital animation state before getting stunned, ensuring that it can move again
        npc.SetAnimationBool(npc.stunParam, false);
        npc.agent.isStopped = false;
        stunTimer = 0;
    }
}
