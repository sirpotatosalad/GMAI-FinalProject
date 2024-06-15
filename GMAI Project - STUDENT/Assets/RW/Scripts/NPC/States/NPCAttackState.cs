using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCAttackState : NPCState
{

    private float attackTimer;

    private Coroutine attackCoroutine;

    public NPCAttackState(NPCController npc, NPCStateMachine stateMachine) : base(npc, stateMachine) { }

    public override void Enter()
    {
        base.Enter();
        Debug.Log("NPC - Entered state: ATTACK");
        npc.agent.isStopped = true;

        AttackPlayer();

        attackTimer = npc.attackDelay;
        
    }

    private void AttackPlayer()
    {
        npc.LookAtPlayer();
        npc.TriggerAnimation(npc.attackParam);

        if (attackCoroutine != null)
        {
            npc.StopCoroutine(attackCoroutine);
        }

        npc.StartCoroutine(HandleHitBox());
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if(!npc.IsPlayerInRange(npc.attackRange))
        {
            stateMachine.ChangeState(npc.chase);
        }

        attackTimer -= Time.deltaTime;

        if (attackTimer <= 0f) 
        {
            AttackPlayer();

            attackTimer = npc.attackDelay;
        }

    }

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
        npc.agent.isStopped = false;
    }

}
