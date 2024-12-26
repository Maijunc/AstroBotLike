using UnityEngine;
public abstract class EnemyBaseState : IState 
{
    protected readonly Enemy enemy;
    protected readonly Animator animator;

    // Get Animator Hash
    protected static readonly int IdleHash = Animator.StringToHash("IdleNormal");
    protected static readonly int RunHash = Animator.StringToHash("RunFWD");

    // walk hash
    protected static readonly int WalkHash = Animator.StringToHash("WalkFWD");
    // attack hash
    protected static readonly int AttackHash = Animator.StringToHash("Attack01");
    // die hash
    protected static readonly int DieHash = Animator.StringToHash("Die");


    protected const float crossFadeDuration = 0.1f;
    protected EnemyBaseState(Enemy enemy, Animator animator) 
    {
        this.enemy = enemy;
        this.animator = animator;
    }

    public virtual void OnEnter() { }
    public virtual void Update() { }
    public virtual void FixedUpdate() { }
    public virtual void OnExit() { }
}