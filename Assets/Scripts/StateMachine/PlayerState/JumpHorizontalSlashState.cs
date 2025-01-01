using UnityEngine;

public class JumpHorizontalSlashState : BaseState
{
    public JumpHorizontalSlashState(PlayerController player, Animator animator) : base(player, animator)
    {
    }

    public override void OnEnter() 
    {
        // Debug.Log("Enter Jump HorizontalSlashState");
        animator.CrossFadeInFixedTime(HorizontalSlashHash, 0.1f, layer: 1);
    }

    public override void FixedUpdate()
    {
        player.HandleJump();
        player.HandleHorizontalSlash();
        player.HandleMovement();
    }
}

