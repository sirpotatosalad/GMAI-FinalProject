using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RayWenderlich.Unity.StatePatternInUnity
{
    public class DrawState : State
    {
        // similar bool initialisation as previously provided state scripts
        private bool swing = false;
        private bool sheath = false;
        private bool block = false;
        private int drawParam = Animator.StringToHash("DrawMelee");
        // Start is called before the first frame update
        public DrawState(Character character, StateMachine stateMachine) : base(character, stateMachine) { }

        public override void Enter()
        {
            base.Enter();
            Debug.Log("Entered state: DRAW");
            DrawWeapon();
        }

        // handle weapon drawing
        private void DrawWeapon()
        {
            // make use of provided Equip method, attaching the assigned melee weapon to the player's hand transform (provided by project initially)
            // similar logic to Sheath state - i.e. triggering animations and playing sound
            character.Equip(character.MeleeWeapon);
            character.TriggerAnimation(drawParam);
            SoundManager.Instance.PlaySound(SoundManager.Instance.meleeEquip);
        }

        // similar logic applies to HandleInput as previously provided states in LMS tutorial
        // i.e. assigning bools to player keyboard inputs
        public override void HandleInput()
        {
            base.HandleInput();
            swing = Input.GetKeyDown(KeyCode.F);
            sheath = Input.GetKeyDown(KeyCode.R);
            // player isn't necessarily blocking in this state, rather its to prevent them from transitioning to swing state
            // i.e. prevent player from attacking when blocking
            block = Input.GetButton("Fire2");
        }

        public override void LogicUpdate()
        {
            // transition to respective states based on player input from keyboard
            base.LogicUpdate();
            if (sheath)
            {
                stateMachine.ChangeState(character.sheathSword);
            }
            else if (swing && !block)
            {
                stateMachine.ChangeState(character.swingSword);
            }
        }
    }
}

