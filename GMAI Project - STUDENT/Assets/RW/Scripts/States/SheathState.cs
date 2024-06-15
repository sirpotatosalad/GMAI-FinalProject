using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RayWenderlich.Unity.StatePatternInUnity
{
    public class SheathState : State
    {

        private bool draw = false;
        private int sheathParam = Animator.StringToHash("SheathMelee");

        public SheathState(Character character, StateMachine stateMachine) : base(character, stateMachine) { }

        public override void Enter()
        {
            base.Enter();
            Debug.Log("Entered state: SHEATH");
            SheathWeapon();
        }

        private void SheathWeapon()
        {
            // play sounds and trigger the sheathing animation in animator
            SoundManager.Instance.PlaySound(SoundManager.Instance.meleeSheath);
            character.TriggerAnimation(sheathParam);
            // make use of provided method in Character to parent sword to player's back
            character.SheathWeapon();

        }

        public override void HandleInput()
        {
            base.HandleInput();
            // handle input for draw/sheath input from player, in this case R key
            draw = Input.GetKeyDown(KeyCode.R);
            
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();
            // check if sword is drawn using keyboard
            if (draw)
            {
                // use provided unequip method to remove sword from hand upon exiting state
                // if this isn't done the player will "magically" conjure up a new sword in the next state!
                // afterwards, transition to Draw state
                character.Unequip();
                stateMachine.ChangeState(character.drawSword);
            }
        }

    }

    
}
