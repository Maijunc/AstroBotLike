using UnityEngine;
using UnityEngine.AI;

public class TurleDashState : EnemyBaseState {
    readonly NavMeshAgent agent;
    readonly Transform player;
    readonly float dashSpeed;
    public TurleDashState(Enemy enemy, Animator animator, NavMeshAgent agent, float dashSpeed, Transform player) : base(enemy, animator) 
    {
        this.agent = agent;
        this.dashSpeed = dashSpeed;
        this.player = player;
    }

    public override void Update()
    {
        // 设置敌人的目标点为玩家的位置
        agent.SetDestination(player.position);
    }

    public override void OnEnter()
    {
        Debug.Log("Turtle Dash");
        animator.CrossFade(RunHash, crossFadeDuration);
        agent.speed += dashSpeed;
        agent.acceleration += dashSpeed;
        agent.angularSpeed += 240;
    }

    public override void OnExit()
    {
        agent.speed -= dashSpeed;
        agent.acceleration -= dashSpeed;
        agent.angularSpeed -= 240;
    }
}