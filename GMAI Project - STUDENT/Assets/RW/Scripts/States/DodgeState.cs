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

        // trigger dodge param in anim controller on enter
        public override void Enter()
        {
            base.Enter();
            Debug.Log("Entered state: DODGE");
            dodgeTimer = 0f;
            character.TriggerAnimation(character.dodgeParam);
        }

        // switch back to standing state once dodge is "completed"
        // i.e. timer is used to allow DodgeState's physics update to move the player character forward during dodge
        public override void LogicUpdate()
        {
            base.LogicUpdate();
            
            dodgeTimer += Time.deltaTime;

            if (dodgeTimer >= dodgeDuration)
            {
                stateMachine.ChangeState(character.standing);
            }
        }

        // move player forward during dodge state
        // emulates dodging when done alongside dodging anim triggered above in Enter()
        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
            character.Dodge(character.DodgeDistance);
        }

        public override void Exit()
        {
            base.Exit();
            dodgeTimer = 0f;
        }
    }
}

