using UnityEngine;
using UnityEngine.AI;
using KBCore.Refs;
using System;
using static Timer;
using System.Collections.Generic;
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

    [SerializeField] float deathAnimationDuration = 2f;

    StateMachine stateMachine;

    CountdownTimer attackTimer;
    CountdownTimer deathTimer;

    List<Timer> timers;

    void OnValidate() => this.ValidateRefs();

    void Awake()
    {
        attackTimer = new CountdownTimer(timeBetweenAttacks);
        deathTimer = new CountdownTimer(deathAnimationDuration);

        deathTimer.OnTimerStart += () => {
            Debug.Log("DeathTimer Start");
        };

        deathTimer.OnTimerStop += () => {
            Debug.Log("DeathTimer Stop");
            DeathSequence();
        };

        timers = new List<Timer> { attackTimer, deathTimer };

        stateMachine = new StateMachine();

        var wanderState = new EnemyWanderState(this, animator, agent, wanderRadius);
        var chaseState = new EnemyChaseState(this, animator, agent, playerDetector.Player);
        var attackState = new EnemyAttackState(this, animator, agent, playerDetector.Player);
        var dieState = new EnemyDieState(this, animator);

        At(wanderState, chaseState, new FuncPredicate(() => playerDetector.CanDetectPlayer()));
        At(chaseState, wanderState, new FuncPredicate(() => !playerDetector.CanDetectPlayer()));
        At(chaseState, attackState, new FuncPredicate(() => playerDetector.CanAttackPlayer()));
        At(attackState, chaseState, new FuncPredicate(() => !playerDetector.CanAttackPlayer()));

        Any(dieState, new FuncPredicate(() => deathTimer.IsRunning));
        // At(dieState, wanderState, new FuncPredicate(() => !deathTimer.IsRunning));

        stateMachine.SetState(wanderState);
    }

    void Start()
    {
        
    }

    void At(IState from, IState to, IPredicate condition) => stateMachine.AddTransition(from, to, condition);
    void Any(IState to, IPredicate condition) => stateMachine.AddAnyTransition(to, condition);

    void Update()
    {
        stateMachine.Update();
        HandleTimers();
    }

    void FixedUpdate()
    {
        stateMachine.FixedUpdate();
    }

    private void HandleTimers()
    {
        foreach (var timer in timers)
        {
            timer.Tick(Time.deltaTime);
        }
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

    public void KnockAway(float attackForce)
    {
        // 获取Rigidbody组件
        Rigidbody enemyRb = GetComponent<Rigidbody>();
        if (enemyRb != null)
        {
            // 禁用NavMeshAgent
            NavMeshAgent navAgent = GetComponent<NavMeshAgent>();
            if (navAgent != null)
            {
                navAgent.enabled = false;  // 禁用NavMeshAgent
            }

            // 计算击飞方向
            // 这里假设攻击是从玩家向敌人发起的，所以方向是从玩家到敌人的
            Vector3 direction = (transform.position - playerDetector.Player.transform.position).normalized;

            // 设置一个垂直的力（如果需要的话可以调整该值）
            direction.y = 1f;  // 让敌人有一个向上的力，模拟击飞效果

            // 向敌人应用一个冲击力，模拟被击飞的效果
            enemyRb.AddForce(direction * attackForce, ForceMode.Impulse);

            // 也可以让敌人在击飞时有一定的旋转效果
            float spinForce = 200f;  // 调整旋转的力度
            enemyRb.AddTorque(new Vector3(0, spinForce, 0), ForceMode.Impulse);
        }
    }

    public void Die()
    {
        // 将怪物的标签更改为 "Dead"
        gameObject.tag = "Dead";

        // 启动死亡计时器
        deathTimer.Start();

        // 播放死亡动画，生成死亡特效
        if (deathVFXPrefab != null)
        {
            GameObject vfx = Instantiate(deathVFXPrefab, transform.position, Quaternion.identity);
            Destroy(vfx, animationDuration); // 2秒后销毁特效
        }
            
    }

    private void DeathSequence()
    {
        // Debug.Log("Destroy");
        // 销毁怪物对象
        Destroy(gameObject);
    }
}