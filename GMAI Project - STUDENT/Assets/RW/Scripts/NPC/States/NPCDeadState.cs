using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCDeadState : NPCState
{
    public NPCDeadState(NPCController npc, NPCStateMachine stateMachine): base(npc, stateMachine) { }

    public override void Enter()
    {
        base.Enter();
        Debug.Log("NPC - Dead");
        npc.SetAnimationBool(npc.deadParam, true);
    }
}
