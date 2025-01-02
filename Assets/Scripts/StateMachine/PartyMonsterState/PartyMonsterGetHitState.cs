using UnityEngine;
using UnityEngine.AI;

public class PartyMonsterGetHitState : PartyMonsterBaseState
{
    readonly NavMeshAgent agent;
    float agentSpeedTmp;
    public PartyMonsterGetHitState(PartyMonster enemy, Animator animator, NavMeshAgent agent) : base(enemy, animator)
    {
        this.agent = agent;
    }

    public override void OnEnter()
    {
        // Debug.Log("Attack");
        animator.CrossFade(GetHitHash, crossFadeDuration);
        agentSpeedTmp = agent.speed;
        agent.speed = 0;
    }

    public override void OnExit()
    {
        agent.speed = agentSpeedTmp;
    }
}
