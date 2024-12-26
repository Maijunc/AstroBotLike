using UnityEngine;

public class LaserJumpState : BaseState 
{
    public LaserJumpState(PlayerController player, Animator animator) : base(player, animator) { }

    public override void OnEnter() 
    {
        // Debug.Log("LaserJumpState OnEnter");
        animator.CrossFade(LaserJumpHash, crossFadeDuration);
    }

    public override void FixedUpdate()
    {
        // call Player's jump logic and move logic
        player.HandleLaserJump();
        player.HandleMovement();
    }
} 