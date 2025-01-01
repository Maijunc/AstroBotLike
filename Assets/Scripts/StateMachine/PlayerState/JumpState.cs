using UnityEngine;

public class JumpState : BaseState 
{
    public JumpState(PlayerController player, Animator animator) : base(player, animator) { }

    public override void OnEnter() 
    {
        // Debug.Log("JumpState OnEnter");
        animator.CrossFade(JumpHash, crossFadeDuration);
        animator.CrossFadeInFixedTime(IdleHash, 0.1f, layer: 1); // 播放上半身攻击动画
    }

    public override void FixedUpdate()
    {
        // call Player's jump logic and move logic
        player.HandleJump();
        player.HandleMovement();
    }
} 
