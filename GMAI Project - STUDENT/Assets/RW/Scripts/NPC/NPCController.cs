using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.AI;



public class NPCController : MonoBehaviour
{

    public NavMeshAgent agent;

    public NPCStateMachine stateMachine;
    public NPCPatrolState standing;

    private int horizonalMoveParam = Animator.StringToHash("H_Speed");
    private int verticalMoveParam = Animator.StringToHash("V_Speed");
    // Start is called before the first frame update
    void Start()
    {
        stateMachine = new NPCStateMachine();

        standing = new NPCPatrolState(this, stateMachine);

        stateMachine.Initialize(standing);
    }

    // Update is called once per frame
    void Update()
    {
        stateMachine.CurrentState.HandleInput();
        stateMachine.CurrentState.LogicUpdate();
    }

    private void FixedUpdate()
    {
       stateMachine.CurrentState.PhysicsUpdate();
    }
}


