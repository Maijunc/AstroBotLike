using UnityEngine;
using UnityEngine.AI;

public class EnemyChaseState : EnemyBaseState
{
    readonly NavMeshAgent agent;
    readonly Transform player;

    public EnemyChaseState(Enemy enemy, Animator animator, NavMeshAgent agent, Transform player) : base(enemy, animator)
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
