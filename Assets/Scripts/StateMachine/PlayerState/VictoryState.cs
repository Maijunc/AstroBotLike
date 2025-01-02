using UnityEngine;

public class VictoryState : BaseState
{
    public VictoryState(PlayerController player, Animator animator) : base(player, animator)
    {
    }

    public override void OnEnter() 
    {
        animator.CrossFade(VictoryHash, crossFadeDuration);
    }
}