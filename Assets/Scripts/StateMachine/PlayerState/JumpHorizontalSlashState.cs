using UnityEngine;

public class JumpHorizontalSlashState : BaseState
{
    public JumpHorizontalSlashState(PlayerController player, Animator animator) : base(player, animator)
    {
    }

    public override void OnEnter() 
    {
        // 获取当前层的动画状态信息
        AnimatorStateInfo currentState = animator.GetCurrentAnimatorStateInfo(1); // layer 1 代表你设置的动画层
        // 检查当前动画状态是否已经是目标动画状态
        if (!currentState.IsName("HorizontalSlash")) 
            animator.CrossFadeInFixedTime(HorizontalSlashHash, 0.1f, layer: 1);
        
    }

    public override void FixedUpdate()
    {
        player.HandleJump();
        player.HandleHorizontalSlash();
        player.HandleMovement();
    }
}

