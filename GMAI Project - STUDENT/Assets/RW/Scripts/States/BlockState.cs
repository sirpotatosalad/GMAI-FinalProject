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

        // set anim bools for blocking
        public override void Enter()
        {
            base.Enter();
            character.SetAnimationBool(character.blockParam, true);
            Debug.Log("Entered state: BLOCK");
        }

        // similar to ducking state, checking if block keybind is held down
        public override void HandleInput()
        {
            base.HandleInput();
            blockHeld = Input.GetButton("Fire2");
        }

        // change state once block keybind is released
        public override void LogicUpdate()
        {
            base.LogicUpdate();
            if (!blockHeld)
            {
                stateMachine.ChangeState(character.standing);
            }
        }

        // disable (set false) blocking bools when exiting block state
        public override void Exit()
        {
            base.Exit();
            character.SetAnimationBool(character.blockParam, false);
        }
    }
}
