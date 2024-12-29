using UnityEngine;

public class JumpState : BaseState 
{
    public JumpState(PlayerController player, Animator animator) : base(player, animator) { }

    public override void OnEnter() 
    {
        // Debug.Log("JumpState OnEnter");
        animator.CrossFade(JumpHash, crossFadeDuration);
    }

    public override void FixedUpdate()
    {
        // call Player's jump logic and move logic
        player.HandleJump();
        player.HandleMovement();
    }
} 
