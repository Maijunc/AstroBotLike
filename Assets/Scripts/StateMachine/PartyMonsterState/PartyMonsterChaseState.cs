using UnityEngine;

public class PartyMonsterChaseState : PartyMonsterBaseState
{
    readonly UnityEngine.AI.NavMeshAgent agent;
    readonly Transform player;

    public PartyMonsterChaseState(PartyMonster enemy, Animator animator, UnityEngine.AI.NavMeshAgent agent, Transform player) : base(enemy, animator)
    {
        this.agent = agent;
        this.player = player;
    }

    public override void OnEnter()
    {
        // Debug.Log("Chase");
        animator.CrossFade(RunHash, crossFadeDuration);
    }

    public override void Update()
    {
        // 设置敌人的目标点为玩家的位置
        agent.SetDestination(player.position);
    }
}
