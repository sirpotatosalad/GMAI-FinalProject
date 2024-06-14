using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class NPCState
{
    protected NPCController npc;
    protected NPCStateMachine stateMachine;

    protected NPCState(NPCController npc, NPCStateMachine stateMachine)
    {
        this.npc = npc;
        this.stateMachine = stateMachine;
    }

    public virtual void Enter()
    {

    }
    public virtual void HandleInput()
    {

    }
    public virtual void LogicUpdate()
    {

    }
    public virtual void PhysicsUpdate()
    {

    }
    public virtual void Exit()
    {

    }
}