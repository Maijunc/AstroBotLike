using UnityEngine;


public class PartyMonsterAttackState : PartyMonsterBaseState
{
    readonly UnityEngine.AI.NavMeshAgent agent;
    readonly Transform player;
    public PartyMonsterAttackState(PartyMonster enemy, Animator animator, UnityEngine.AI.NavMeshAgent agent, Transform player) : base(enemy, animator)
    {
        this.agent = agent;
        this.player = player;
    }

    public override void OnEnter()
    {
        enemy.canAttack = true;
        // Debug.Log("Attack");
        animator.CrossFade(AttackHash, crossFadeDuration);
    }

    public override void Update()
    {
        agent.SetDestination(player.position);

        if (enemy.canAttack)
            enemy.Attack();
    }
}
