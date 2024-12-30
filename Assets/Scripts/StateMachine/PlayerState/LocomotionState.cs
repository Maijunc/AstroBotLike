using UnityEngine;

public class LocomotionState : BaseState 
{
    public LocomotionState(PlayerController player, Animator animator) : base(player, animator) 
    {

    }

    public override void OnEnter() 
    {
        // Debug.Log("LocomotionState OnEnter");
        animator.CrossFade(LocomotionHash, crossFadeDuration);
        animator.CrossFadeInFixedTime("Idle", 0.1f, layer: 1); // 播放上半身攻击动画
    }

    public override void FixedUpdate()
    {
        // call Player's move logic 
        player.HandleMovement();
    }
}

