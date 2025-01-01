using UnityEngine;

public class JumpChargeState : BaseState
{
    public JumpChargeState(PlayerController player, Animator animator) : base(player, animator)
    {
    }

    public override void OnEnter()
    {
        // Debug.Log("Enter JumpChargeState");
        animator.CrossFadeInFixedTime(ChargeHash, 0.1f, layer: 1);
    }

    public override void FixedUpdate()
    {
        player.HandleJump();
        player.HandleCharge();
        player.HandleMovement();
    }
}