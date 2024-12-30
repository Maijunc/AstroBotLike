using UnityEngine;

public class SpinAttackState : BaseState
{
    public SpinAttackState(PlayerController player, Animator animator) : base(player, animator)
    {
    }
    public override void OnEnter() 
    {
        // Debug.Log("Enter SpinAttackState");
        animator.CrossFade(SpinAttackHash, crossFadeDuration);
    }

    public override void FixedUpdate()
    {
        player.HandleSpinAttack();
        player.HandleMovement();
    }
} 
