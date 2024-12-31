using UnityEngine;

public class EnemyDieState : EnemyBaseState
{
    public EnemyDieState(Enemy enemy, Animator animator) : base(enemy, animator)
    {
    }

    public override void OnEnter()
    {
        // Debug.Log("Attack");
        animator.CrossFade(DieHash, crossFadeDuration);
    }
}
