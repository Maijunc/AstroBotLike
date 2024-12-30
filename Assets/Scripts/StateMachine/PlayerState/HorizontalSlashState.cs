using UnityEngine;

public class HorizontalSlashState : BaseState
{
    public HorizontalSlashState(PlayerController player, Animator animator) : base(player, animator)
    {
        
    }

    public override void OnEnter() 
    {
        // Debug.Log("Enter HorizontalSlashState");
        animator.CrossFadeInFixedTime(HorizontalSlashHash, 0.1f, layer: 1);
    }

    public override void FixedUpdate()
    {
        player.HandleHorizontalSlash();
        player.HandleMovement();
    }
} 
