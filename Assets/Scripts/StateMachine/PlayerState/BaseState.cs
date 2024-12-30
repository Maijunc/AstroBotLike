using UnityEngine;

public abstract class BaseState : IState {
    protected readonly PlayerController player;
    protected readonly Animator animator;

    // 动画状态机参数 用Hash传递，更快，更准
    protected static readonly int LocomotionHash = Animator.StringToHash("Locomotion");
    protected static readonly int AttackLocomotionHash = Animator.StringToHash("AttackLocomotion");
    protected static readonly int JumpHash = Animator.StringToHash("Jump");
    protected static readonly int LaserJumpHash = Animator.StringToHash("LaserJump");
    protected static readonly int DashHash = Animator.StringToHash("Dash");
    protected static readonly int SpinAttackHash = Animator.StringToHash("SpinAttack");
    protected static readonly int HorizontalSlashHash = Animator.StringToHash("HorizontalSlash");
    protected static readonly int DiagonalSlashHash = Animator.StringToHash("DiagonalSlash");
    protected static readonly int ChargeHash = Animator.StringToHash("Charge");

    // 动画过渡时间
    protected const float crossFadeDuration = 0.1f;

    protected BaseState(PlayerController player, Animator animator) {
        this.player = player;
        this.animator = animator;
    }
    public virtual void OnEnter() 
    {
    }
    public virtual void Update() 
    {

    }
    public virtual void FixedUpdate() 
    {

    }
    public virtual void OnExit() 
    {
        // Debug.Log("BaseState OnExit");
    }
}
