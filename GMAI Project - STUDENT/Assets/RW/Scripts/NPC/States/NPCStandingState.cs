using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCPatrolState : NPCState
{
    public NPCPatrolState(NPCController npc, NPCStateMachine stateMachine) : base(npc, stateMachine) { }

    public override void Enter()
    {
        base.Enter();
        Debug.Log("NPC - Entered state: STANDING");
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

    }

}
