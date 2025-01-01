using UnityEngine;

public class JumpDiagonalSlashState : BaseState
{
    public JumpDiagonalSlashState(PlayerController player, Animator animator) : base(player, animator)
    {
    }

    public override void OnEnter() 
    {
        // Debug.Log("Enter Jump DiagonalSlashState");
        animator.CrossFadeInFixedTime(DiagonalSlashHash, 0.1f, layer: 1);
    }

    public override void FixedUpdate()
    {
        player.HandleJump();
        player.HandleDiagonalSlash();
        player.HandleMovement();
    }
}