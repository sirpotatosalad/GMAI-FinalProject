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
            // as usual, set the character's sprint parameter to true
            character.SetAnimationBool(character.sprintParam, true);
            // modify character movement and rotation speed to more closely resemble sprinting
            speed = character.MovementSpeed * 2.1f;
            rotationSpeed = character.RotationSpeed * 0.75f;
        }

        public override void HandleInput()
        {
            // get keyboard inputs for state transitions
            base.HandleInput();
            sprintHeld = Input.GetKey(KeyCode.LeftControl);
            jump = Input.GetButtonDown("Jump");
        }

        public override void LogicUpdate()
        {
            // handle input logic and transitions to other states
            // transition to jump state
            base.LogicUpdate();
            if (jump)
            {
                stateMachine.ChangeState(character.jumping);
            }
            // transition back to standing state when sprint isnt held down / player isnt moving
            else if (!sprintHeld || character.GetComponent<Rigidbody>().velocity == Vector3.zero)
            {
                stateMachine.ChangeState(character.standing);
            }
        }

        public override void Exit()
        {
            base.Exit();
            // set animator parameter to false when exiting state
            character.SetAnimationBool(character.sprintParam, false);
        }
    }
}

