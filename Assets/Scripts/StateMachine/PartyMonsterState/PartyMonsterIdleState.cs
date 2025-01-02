using UnityEngine;
using UnityEngine.AI;

public class PartyMonsterIdleState : PartyMonsterBaseState
{
    readonly NavMeshAgent agent;
    public PartyMonsterIdleState(PartyMonster enemy, Animator animator, NavMeshAgent agent) : base(enemy, animator) { this.agent = agent; }

    public override void OnEnter()
    {
        animator.CrossFade(IdleHash, crossFadeDuration);
        agent.isStopped = true;
    }

    // OnExit
    public override void OnExit()
    {
        agent.isStopped = false;
    }
}
