using UnityEngine;

public class JumpDiagonalSlashState : BaseState
{
    public JumpDiagonalSlashState(PlayerController player, Animator animator) : base(player, animator)
    {
    }

    public override void OnEnter() 
    {
        // 获取当前层的动画状态信息
        AnimatorStateInfo currentState = animator.GetCurrentAnimatorStateInfo(1); // layer 1 代表你设置的动画层
        // Debug.Log("Enter Jump DiagonalSlashState");
        if (!currentState.IsName("DiagonalSlash")) 
            animator.CrossFadeInFixedTime(DiagonalSlashHash, 0.1f, layer: 1);
    }

    public override void FixedUpdate()
    {
        player.HandleJump();
        player.HandleDiagonalSlash();
        player.HandleMovement();
    }
}