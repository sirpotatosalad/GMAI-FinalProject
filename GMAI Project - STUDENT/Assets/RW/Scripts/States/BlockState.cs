using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace RayWenderlich.Unity.StatePatternInUnity
{
    public class BlockState : GroundedState
    {
        public bool blockHeld;
        public BlockState(Character character, StateMachine stateMachine) : base(character, stateMachine) { }

        public override void Enter()
        {
            base.Enter();
            isBlocking = true;
            character.SetAnimationBool(character.blockParam, true);
            Debug.Log("Entered state: BLOCK");
        }

        public override void HandleInput()
        {
            base.HandleInput();
            blockHeld = Input.GetButton("Fire2");
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();
            if (!blockHeld)
            {
                stateMachine.ChangeState(character.standing);
            }
        }

        public override void Exit()
        {
            base.Exit();
            isBlocking = false;
            character.SetAnimationBool(character.blockParam, false);
        }
    }
}
