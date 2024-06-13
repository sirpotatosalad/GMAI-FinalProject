using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace RayWenderlich.Unity.StatePatternInUnity
{
    public class SprintState : GroundedState
    {
        private bool sprintHeld;
        private bool jump;

        public SprintState(Character character, StateMachine stateMachine) : base(character, stateMachine) { }
        // Start is called before the first frame update
        public override void Enter()
        {
            base.Enter();
            Debug.Log("Entered state: SPRINT");
            character.SetAnimationBool(character.sprintParam, true);
            speed = character.MovementSpeed * 1.5f;
            rotationSpeed = character.RotationSpeed * 0.75f;
        }

        public override void HandleInput()
        {
            base.HandleInput();
            sprintHeld = Input.GetKey(KeyCode.LeftControl);
            jump = Input.GetButtonDown("Jump");
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();
            if (jump)
            {
                stateMachine.ChangeState(character.jumping);
            }
            else if (!sprintHeld)
            {
                stateMachine.ChangeState(character.standing);
            }
        }

        public override void Exit()
        {
            base.Exit();
            character.SetAnimationBool(character.sprintParam, false);
        }
    }
}

