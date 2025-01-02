using UnityEngine;

public class PartyMonsterSlideState : PartyMonsterBaseState
{
    readonly UnityEngine.AI.NavMeshAgent agent;
    readonly Transform player;
    readonly PlayerDetector playerDetector;
    readonly float initialAttackRange;
    readonly float maxAttackRange;
    readonly float slideSpeed;
    public PartyMonsterSlideState(PartyMonster enemy, Animator animator, UnityEngine.AI.NavMeshAgent agent, Transform player, PlayerDetector playerDetector, float initialAttackRange, float maxAttackRange, float slideSpeed) : base(enemy, animator)
    {
        this.agent = agent;
        this.player = player;
        this.playerDetector = playerDetector;
        this.initialAttackRange = initialAttackRange;
        this.maxAttackRange = maxAttackRange;
        this.slideSpeed = slideSpeed;
    }

    public override void OnEnter()
    {
        enemy.canAttack = true;
        // Debug.Log("Attack");
        animator.CrossFade(SlideHash, crossFadeDuration);
        // agent.isStopped = true;
        enemy.slideTimer.Start();

        agent.speed += slideSpeed;
    }

    public override void Update()
    {
        if(!enemy.canAttack || (1 - enemy.slideTimer.Progress) >= 0.8f)
            return;
        // 计算当前攻击范围，基于倒计时进度
        float currentAttackRange = Mathf.Lerp(initialAttackRange, maxAttackRange, 1 - enemy.slideTimer.Progress);

        agent.SetDestination(player.position);
        // 检测玩家是否在当前攻击范围内
        if (playerDetector.CanAttackPlayer(currentAttackRange))
            enemy.Attack();
    }

    public override void OnExit()
    {
        // agent.isStopped = false;
        agent.speed -= slideSpeed;
    }
}
