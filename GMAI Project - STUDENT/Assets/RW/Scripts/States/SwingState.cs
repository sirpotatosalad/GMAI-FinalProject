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

        private Coroutine swingCoroutine;
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

            if (swingCoroutine != null)
            {
                character.StopCoroutine(swingCoroutine);
            }

            character.StartCoroutine(HandleHitBox());
        }

        private IEnumerator HandleHitBox()
        {
            float activateTime = 0.2f;
            float deactivateTime = 0.5f;

            yield return new WaitForSeconds(activateTime);
            character.ActivateHitBox();

            yield return new WaitForSeconds(deactivateTime);
            character.DeactivateHitBox();
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
