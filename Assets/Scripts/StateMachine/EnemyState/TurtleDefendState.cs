using UnityEngine;
using UnityEngine.AI;

public class TurtleDefendState : EnemyBaseState
{
    readonly NavMeshAgent agent;
    public TurtleDefendState(Enemy enemy, Animator animator, NavMeshAgent agent) : base(enemy, animator)
    {
        this.agent = agent;
    }

    public override void OnEnter()
    {
        // Debug.Log("Defend");
        animator.CrossFade(DefendHash, crossFadeDuration);
        StartDefend();
    }

    public override void OnExit()
    {
        EndDefend();
    }

    private void StartDefend()
    {
        Debug.Log("Turtle enters defense mode!");
        
        // 停止移动或其他行为
        if(agent.isOnNavMesh)
            agent.isStopped = true;
    }

    private void EndDefend()
    {
        Debug.Log("Turtle exits defense mode!");

        // 恢复移动或其他行为
        if(agent.isOnNavMesh)
            agent.isStopped = false;
    }
}
