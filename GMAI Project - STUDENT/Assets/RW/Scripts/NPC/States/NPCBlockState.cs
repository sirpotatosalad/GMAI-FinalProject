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
        npc.TriggerAnimation(npc.blockParam);
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (!npc.IsAnimatorPlaying(0, "Block"))
        {
            stateMachine.ChangeState(npc.attack);
        }
    }
}
