using UnityEngine;
using UnityEngine.AI;
using KBCore.Refs;
using System;
using static Timer;
public class Enemy : Entity
{
    [SerializeField, Self] NavMeshAgent agent;
    [SerializeField, Self] PlayerDetector playerDetector;
    [SerializeField, Child] Animator animator;

    // 特效
    // [SerializeField] GameObject spawnVFXPrefab;
    [SerializeField] GameObject deathVFXPrefab; // 死亡特效预制体
    [SerializeField] float animationDuration = 2f;

    [SerializeField] float wanderRadius = 5f;
    [SerializeField] float timeBetweenAttacks = 1f;

    StateMachine stateMachine;

    CountdownTimer attackTimer;

    void OnValidate() => this.ValidateRefs();

    void Start()
    {
        attackTimer = new CountdownTimer(timeBetweenAttacks);
        stateMachine = new StateMachine();

        var wanderState = new EnemyWanderState(this, animator, agent, wanderRadius);
        var chaseState = new EnemyChaseState(this, animator, agent, playerDetector.Player);
        var attackState = new EnemyAttackState(this, animator, agent, playerDetector.Player);

        At(wanderState, chaseState, new FuncPredicate(() => playerDetector.CanDetectPlayer()));
        At(chaseState, wanderState, new FuncPredicate(() => !playerDetector.CanDetectPlayer()));
        At(chaseState, attackState, new FuncPredicate(() => playerDetector.CanAttackPlayer()));
        At(attackState, chaseState, new FuncPredicate(() => !playerDetector.CanAttackPlayer()));

        stateMachine.SetState(wanderState);
    }

    void At(IState from, IState to, IPredicate condition) => stateMachine.AddTransition(from, to, condition);
    void Any(IState to, IPredicate condition) => stateMachine.AddAnyTransition(to, condition);

    void Update()
    {
        stateMachine.Update();
        attackTimer.Tick(Time.deltaTime);
    }

    void FixedUpdate()
    {
        stateMachine.FixedUpdate();
    }

    public void Attack()
    {
        if (attackTimer.IsRunning) return;

        attackTimer.Start();
        playerDetector.Player.GetComponent<PlayerController>().TakeDamage(1);
    }

    public void TakeDamage(float damage)
    {
        GetComponent<Health>().TakeDamage((int)damage);
        if(GetComponent<Health>().currentHealth <= 0)
            Die();
    }

    private void Die()
    {
        // 播放死亡动画
        // 在怪物位置生成死亡特效
        if (deathVFXPrefab != null)
        {
            GameObject vfx = Instantiate(deathVFXPrefab, transform.position, Quaternion.identity);
            Destroy(vfx, animationDuration); // 2秒后销毁特效
        }

        // 销毁怪物对象
        Destroy(gameObject); // 或者播放死亡动画后销毁
            
    }
}