using UnityEngine;

public class DeathState : BaseState
{
    public DeathState(PlayerController player, Animator animator) : base(player, animator){    }

    public override void OnEnter() 
    {
        Debug.Log("Player die");
        animator.CrossFadeInFixedTime(IdleHash, 0.1f, layer: 1); // 播放上半身攻击动画
        animator.CrossFade(DeathHash, crossFadeDuration);
    }
}