using UnityEngine;

public class DiagonalSlashState : BaseState
{
    public DiagonalSlashState(PlayerController player, Animator animator) : base(player, animator)
    {
    }

    public override void OnEnter() 
    {
        // Debug.Log("Enter DiagonalSlashState");
        animator.CrossFadeInFixedTime(DiagonalSlashHash, 0.1f, layer: 1);
    }

    public override void FixedUpdate()
    {
        player.HandleDiagonalSlash();
        player.HandleMovement();
    }
} 
