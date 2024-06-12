using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace RayWenderlich.Unity.StatePatternInUnity
{
    public class DodgeState : GroundedState
    {

        public DodgeState(Character character, StateMachine stateMachine) : base(character, stateMachine) { }

        public override void Enter()
        {
            base.Enter();
            Debug.Log("Entered state: DODGE");
            Dodge();
            isDodging = true;
        }

        private void Dodge()
        {
            character.TriggerAnimation(character.dodgeParam);
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();
            if (character.IsAnimatorPlaying(0, "Dodge"))
            {
                isDodging = false;
                stateMachine.ChangeState(character.standing);
            }
        }

        public override void Exit()
        {
            base.Exit();
        }
    }
}

