using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RayWenderlich.Unity.StatePatternInUnity
{
    public class SwingState : State
    {
        private int swingParam = Animator.StringToHash("SwingMelee");
        private bool swing = false;
        private bool sheath = false;
        private bool block = false;
        // Start is called before the first frame update
        public SwingState(Character character, StateMachine stateMachine) : base(character, stateMachine) { }

        public override void Enter()
        {
            base.Enter();
            Debug.Log("Entered state: SWING");
            SwingWeapon();
        }

        private void SwingWeapon()
        {
            SoundManager.Instance.PlaySound(SoundManager.Instance.meleeSwings);
            character.TriggerAnimation(swingParam);
        }

        public override void HandleInput()
        {
            base.HandleInput();
            swing = Input.GetKeyDown(KeyCode.F);
            sheath = Input.GetKeyDown(KeyCode.R);
            block = Input.GetButton("Fire2");
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();

            if (swing && !character.IsAnimatorPlaying(1, "SwingSword") && !block)
            {
               SwingWeapon();
            }
            else if (sheath)
            {
                stateMachine.ChangeState(character.sheathSword);
            }

        }
    }
}
