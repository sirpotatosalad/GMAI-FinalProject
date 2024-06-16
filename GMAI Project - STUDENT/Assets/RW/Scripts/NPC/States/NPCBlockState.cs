using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCBlockState : NPCState
{
    public NPCBlockState(NPCController npc, NPCStateMachine stateMachine) : base(npc, stateMachine) { }

    public override void Enter()
    {
        base.Enter();
        Debug.Log("NPC - Attack blocked");
        // trigger NPC's blocking animation upon entering state
        npc.TriggerAnimation(npc.blockParam);
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        // transition back to attack state after block animation finishes playing 
        if (!npc.IsAnimatorPlaying(0, "Block"))
        {
            stateMachine.ChangeState(npc.attack);
        }
    }
}
