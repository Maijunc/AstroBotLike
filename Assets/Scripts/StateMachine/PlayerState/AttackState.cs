using UnityEngine;

public class AttackState : BaseState
{
    public AttackState(PlayerController player, Animator animator) : base(player, animator)
    {
    }

    public override void OnEnter()
    {
        // Debug.Log("AttackState OnEnter");
        animator.CrossFade(AttackLocomotionHash, crossFadeDuration);
        animator.CrossFadeInFixedTime("Attack", 0.1f, layer: 1); // 播放上半身攻击动画
        player.Attack();
    }

    public override void FixedUpdate()
    {
        player.HandleMovement();
    }
}
