using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RayWenderlich.Unity.StatePatternInUnity
{
    public class SheathState : State
    {

        private bool onFirstStart = true;
        // Start is called before the first frame update
        public SheathState(Character character, StateMachine stateMachine) : base(character, stateMachine) { }

        public override void Enter()
        {
            base.Enter();
            Debug.Log("Entered state: SHEATH");
            SoundManager.Instance.PlaySound(SoundManager.Instance.meleeSheath);

        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();
            if (Input.GetKeyDown(KeyCode.R))
            {
                stateMachine.ChangeState(character.drawSword);
                Debug.Log("Drawing weapon");
            }
        }

    }

    
}
