using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace RayWenderlich.Unity.StatePatternInUnity
{
    public class DodgeState : GroundedState
    {
        private float dodgeTimer;
        private float dodgeDuration = 1.2f;

        public DodgeState(Character character, StateMachine stateMachine) : base(character, stateMachine) { }

        public override void Enter()
        {
            base.Enter();
            Debug.Log("Entered state: DODGE");
            dodgeTimer = 0f;
            character.TriggerAnimation(character.dodgeParam);
        }


        public override void LogicUpdate()
        {
            base.LogicUpdate();
            
            dodgeTimer += Time.deltaTime;

            if (dodgeTimer >= dodgeDuration)
            {
                stateMachine.ChangeState(character.standing);
            }
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
            character.Dodge(character.DodgeDistance);
        }

        public override void Exit()
        {
            base.Exit();
            dodgeTimer = 0f;
            isDodging = false;
        }
    }
}

