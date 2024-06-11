using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RayWenderlich.Unity.StatePatternInUnity
{
    public class DrawState : State
    {

        private bool sheath = false;
        private bool swing = false;
        private int drawParam = Animator.StringToHash("DrawMelee");
        // Start is called before the first frame update
        public DrawState(Character character, StateMachine stateMachine) : base(character, stateMachine) { }

        public override void Enter()
        {
            base.Enter();
            Debug.Log("Entered state: DRAW");
            DrawWeapon();
        }

        private void DrawWeapon()
        {
            character.Equip(character.MeleeWeapon);
            character.TriggerAnimation(drawParam);
            SoundManager.Instance.PlaySound(SoundManager.Instance.meleeEquip);
        }

        public override void HandleInput()
        {
            base.HandleInput();
            sheath = Input.GetKeyDown(KeyCode.R);
            swing = Input.GetButtonDown("Fire1");
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();
            if (sheath)
            {
                stateMachine.ChangeState(character.sheathSword);
            }
            else if (swing)
            {
                stateMachine.ChangeState(character.swingSword);
            }
        }
    }
}

