using UnityEngine;

public class PartyMonsterDieState : PartyMonsterBaseState
{
    public PartyMonsterDieState(PartyMonster enemy, Animator animator) : base(enemy, animator)
    {
    }

    public override void OnEnter()
    {
        // Debug.Log("Attack");
        animator.CrossFade(DieHash, crossFadeDuration);
    }
}
