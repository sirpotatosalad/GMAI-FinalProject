using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// essentially the provided StateMachine class copied over for use in the NPC FSM
public class NPCStateMachine
{

    public NPCState CurrentState { get; private set; }


    public void Initialize(NPCState startingState)
    {
        CurrentState = startingState;
        startingState.Enter();
    }

    public void ChangeState(NPCState newState)
    {
        CurrentState.Exit();

        CurrentState = newState;
        newState.Enter();
    }

}
