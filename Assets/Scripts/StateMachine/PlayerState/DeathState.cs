using UnityEngine;

public class DeathState : BaseState
{
    public DeathState(PlayerController player, Animator animator) : base(player, animator)
    {
    }

    public override void OnEnter() 
    {
        Debug.Log("Player die");
        animator.CrossFade(DeathHash, crossFadeDuration);
    }
}