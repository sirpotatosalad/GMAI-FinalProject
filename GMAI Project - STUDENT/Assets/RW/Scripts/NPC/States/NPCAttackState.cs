using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCAttackState : NPCState
{

    private float attackTimer;

    // same logic as player character's SwingState
    // reference to THIS state's coroutine instance
    // i.e. the coroutine managining HandleHitBox
    private Coroutine attackCoroutine;

    public NPCAttackState(NPCController npc, NPCStateMachine stateMachine) : base(npc, stateMachine) { }

    public override void Enter()
    {
        base.Enter();
        Debug.Log("NPC - Entered state: ATTACK");

        // stop NPC from moving before attacking
        npc.agent.isStopped = true;
        AttackPlayer();

        attackTimer = npc.attackDelay;
        
    }

    private void AttackPlayer()
    {
        // rotate NPC to look at player and trigger NPC's attack animation
        npc.LookAtPlayer();
        npc.TriggerAnimation(npc.attackParam);

        // check if the coroutine instance in NPCAttackState managing HandleHitBox is running
        // if so, stop it
        if (attackCoroutine != null)
        {
            npc.StopCoroutine(attackCoroutine);
        }

        // start coroutine to allow timed activation/deactivation of NPC's punch hitbox
        npc.StartCoroutine(HandleHitBox());
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        // transition to chase after the player if they leave the NPC's attack range
        if(!npc.DetectionCone(npc.attackRange))
        {
            stateMachine.ChangeState(npc.chase);
        }

        // essentially, NPC will continually attack the player while in this state
        // below simply makes the NPC attack with a delay
        attackTimer -= Time.deltaTime;

        // allow NPC to attack again after delay is finished counting down
        if (attackTimer <= 0f) 
        {
            AttackPlayer();
            // reset the attack delay
            attackTimer = npc.attackDelay;
        }

    }

    // applied logic is the same and copied from the player character's SwingState
    // as seen below, this coroutine simply activates and deactivates the NPC's attack hitbox
    // i.e. roughly matches the hitbox activating with the NPC's punch
    private IEnumerator HandleHitBox()
    {
        float activateTime = 0.2f;
        float deactivateTime = 0.5f;

        yield return new WaitForSeconds(activateTime);
        npc.ActivateHitBox();

        yield return new WaitForSeconds(deactivateTime);
        npc.DeactivateHitBox();
    }

    public override void Exit()
    {
        base.Exit();
        // ensure that NPC can continue moving outside of this state
        npc.agent.isStopped = false;
    }

}
