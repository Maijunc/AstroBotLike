using UnityEngine;

public abstract class PartyMonsterBaseState : IState
{
    protected readonly PartyMonster enemy;
    protected readonly Animator animator;

    // Get Animator Hash
    protected static readonly int IdleHash = Animator.StringToHash("IdleNormal");
    protected static readonly int RunHash = Animator.StringToHash("RunFWD");
    // attack hash
    protected static readonly int AttackHash = Animator.StringToHash("Attack01");
    protected static readonly int SlideHash = Animator.StringToHash("Slide");
    // die hash
    protected static readonly int DieHash = Animator.StringToHash("Die");
    protected static readonly int DefendHash = Animator.StringToHash("Defend");

    protected static readonly int GetHitHash = Animator.StringToHash("GetHit");


    protected const float crossFadeDuration = 0.1f;
    protected PartyMonsterBaseState(PartyMonster enemy, Animator animator) 
    {
        this.enemy = enemy;
        this.animator = animator;
    }

    public virtual void OnEnter() { }
    public virtual void Update() { }
    public virtual void FixedUpdate() { }
    public virtual void OnExit() { }
}
