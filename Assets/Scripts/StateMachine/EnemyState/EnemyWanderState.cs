using UnityEngine;
using UnityEngine.AI;

public class EnemyWanderState : EnemyBaseState
{
    readonly NavMeshAgent agent;
    Vector3 startPoint { get; }
    readonly float wanderRadius;

    public EnemyWanderState(Enemy enemy, Animator animator, NavMeshAgent agent, float wanderRadius) : base(enemy, animator)
    {
        this.agent = agent;
        this.startPoint = enemy.transform.position;
        this.wanderRadius = wanderRadius;
    }

    public override void OnEnter()
    {
        // Debug.Log("Wander");
        animator.CrossFade(WalkHash, crossFadeDuration);
    }

    public override void Update()
    {
        if (HasReachDestination())
        {
            // randomDirection 需要检测的随机位置
            // wanderRadius 检查的最大半径（即代理可以随机移动的范围）
            var randomDirection = Random.insideUnitSphere * wanderRadius;
            randomDirection += startPoint;
            NavMeshHit hit;
            // NavMesh.SamplePosition 是 Unity 中的一个 API 它用来在指定的位置（randomDirection）附近寻找一个有效的、可以导航的点
            NavMesh.SamplePosition(randomDirection, out hit, wanderRadius, 1);
            var finalPosition = hit.position;

            agent.SetDestination(finalPosition);
        }
    }

    // 如果路径计算完成 且 剩余距离小于停止距离 且 没有路径 或 速度为0
    // remainingDistance 表示代理到达目标所剩的距离 stoppingDistance 表示代理停止的距离
    // 当 remainingDistance 小于等于 stoppingDistance 时，代理已经足够接近目标，可以停止移动了
    // 当代理没有路径时，或者代理的速度为0时，代理已经到达目标
    bool HasReachDestination()
    {
        if(!agent.isActiveAndEnabled)
            return false;
        return !agent.pathPending
                && agent.remainingDistance <= agent.stoppingDistance
                && (!agent.hasPath || agent.velocity.sqrMagnitude == 0f);
    }
}
