using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RayWenderlich.Unity.StatePatternInUnity
{
    public class SwingState : State
    {
        // similar bool initialisation as previously provided state scripts
        private int swingParam = Animator.StringToHash("SwingMelee");
        private bool swing = false;
        private bool sheath = false;
        private bool block = false;

        // reference to THIS state's coroutine instance
        // i.e. the coroutine managining HandleHitBox below
        private Coroutine swingCoroutine;

        public SwingState(Character character, StateMachine stateMachine) : base(character, stateMachine) { }

        public override void Enter()
        {
            base.Enter();
            Debug.Log("Entered state: SWING");
            SwingWeapon();
        }

        // handle weapon swinging  
        private void SwingWeapon()
        {
            // similar logic to other states - handling weapon swing anims and sounds
            SoundManager.Instance.PlaySound(SoundManager.Instance.meleeSwings);
            character.TriggerAnimation(swingParam);
            
            // check if the coroutine instance in SwingState managing HandleHitBox is working
            // if so, stop it
            if (swingCoroutine != null)
            {
                character.StopCoroutine(swingCoroutine);
            }

            // start coroutine to allow timed activation/deactivation of player's sword hitbox
            character.StartCoroutine(HandleHitBox());
        }

        // as seen below, this coroutine simply activates and deactivates the player's sword hitbox
        // i.e. matches the hitbox activating with the player's sword swing
        private IEnumerator HandleHitBox()
        {
            float activateTime = 0.2f;
            float deactivateTime = 0.5f;

            yield return new WaitForSeconds(activateTime);
            character.ActivateHitBox();

            yield return new WaitForSeconds(deactivateTime);
            character.DeactivateHitBox();
        }

        // similar logic applies to HandleInput as previously provided states in LMS tutorial
        // i.e. assigning bools to player keyboard inputs
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

            // allows the player to swing their weapon again after the initial entry "swing" 
            // only will let player attack once the swing animation finishes playing, and if player isn't blocking
            if (swing && !character.IsAnimatorPlaying(1, "SwingSword") && !block)
            {
               SwingWeapon();
            }
            // change state to sheath
            else if (sheath)
            {
                stateMachine.ChangeState(character.sheathSword);
            }

        }
    }
}
