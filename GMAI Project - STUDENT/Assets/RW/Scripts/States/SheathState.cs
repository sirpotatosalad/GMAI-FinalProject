using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RayWenderlich.Unity.StatePatternInUnity
{
    public class SheathState : State
    {

        private bool draw = false;
        private int sheathParam = Animator.StringToHash("SheathMelee");
        // Start is called before the first frame update
        public SheathState(Character character, StateMachine stateMachine) : base(character, stateMachine) { }

        public override void Enter()
        {
            base.Enter();
            Debug.Log("Entered state: SHEATH");
            SheathWeapon();
        }

        private void SheathWeapon()
        {
            SoundManager.Instance.PlaySound(SoundManager.Instance.meleeSheath);
            character.TriggerAnimation(sheathParam);
            character.SheathWeapon();

        }

        public override void HandleInput()
        {
            base.HandleInput();
            draw = Input.GetKeyDown(KeyCode.R);
            
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();
            if (draw)
            {
                character.Unequip();
                stateMachine.ChangeState(character.drawSword);
            }
        }

    }

    
}
