using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RayWenderlich.Unity.StatePatternInUnity
{
    public class DrawState : State
    {

        private bool isDrawn;
        // Start is called before the first frame update
        public DrawState(Character character, StateMachine stateMachine) : base(character, stateMachine) { }

        public override void Enter()
        {
            base.Enter();
            character.Equip(character.MeleeWeapon);
            isDrawn = true;
            Debug.Log("Entered state: DRAW");
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();
            if (Input.GetKeyDown(KeyCode.R))
            {
                stateMachine.ChangeState(character.sheathSword);
            }
        }
    }
}

