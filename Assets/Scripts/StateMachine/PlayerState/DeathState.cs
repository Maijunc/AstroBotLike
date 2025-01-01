using UnityEngine;

public class DeathState : BaseState
{
    Rigidbody rb;
    public DeathState(PlayerController player, Animator animator, Rigidbody rb) : base(player, animator){ this.rb = rb;}

    public override void OnEnter() 
    {
        Debug.Log("Player die");
        // 设置为 kinematic，意味着物体不再受物理引擎控制，但仍然能够响应脚本控制
        rb.isKinematic = true;
        animator.CrossFadeInFixedTime(IdleHash, 0.1f, layer: 1); // 播放上半身攻击动画
        animator.CrossFade(DeathHash, crossFadeDuration);
    }
}