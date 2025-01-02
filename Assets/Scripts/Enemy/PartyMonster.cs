using UnityEngine;
using UnityEngine.AI;
using KBCore.Refs;
using System;
using static Timer;
using System.Collections.Generic;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(PlayerDetector))]
[RequireComponent(typeof(AudioSource))]
public class PartyMonster : Entity, IEnemy
{
    [SerializeField, Self] protected NavMeshAgent agent;
    [SerializeField, Self] protected PlayerDetector playerDetector;
    [SerializeField, Child] protected Animator animator;
    [SerializeField, Child] protected AudioSource audioSource;
    [SerializeField, Self] protected Rigidbody rb;
    [SerializeField] AudioClip knockAwaySound;

    // 特效
    // [SerializeField] GameObject spawnVFXPrefab;
    [SerializeField] GameObject deathVFXPrefab; // 死亡特效预制体
    [SerializeField] float animationDuration = 2f;

    [SerializeField] protected float wanderRadius = 5f;
    [SerializeField] float timeBetweenAttacks = 0.4f;

    [SerializeField] float deathAnimationDuration = 2f;
    [SerializeField] float getHitCooldown = 0.2f;

    StateMachine stateMachine;

    CountdownTimer attackTimer;
    protected CountdownTimer deathTimer;
    CountdownTimer getHitTimer;

    protected List<Timer> timers;

    static readonly int deathProgress = Animator.StringToHash("deathProgress");

    public bool canAttack;

    void OnValidate() => this.ValidateRefs();

    protected virtual void Awake()
    {
        if(playerDetector.Player == null)
        {
            // Debug.Log("Player Transform is null in Enemy.");
            playerDetector.GetPlayer();
        }

        attackTimer = new CountdownTimer(timeBetweenAttacks);
        deathTimer = new CountdownTimer(deathAnimationDuration);
        getHitTimer = new CountdownTimer(getHitCooldown);

        deathTimer.OnTimerStop += () => {
            DeathSequence();
        };

        timers = new List<Timer> { attackTimer, deathTimer, getHitTimer };

        stateMachine = new StateMachine();

        var idleState = new PartyMonsterIdleState(this, animator, agent);
        var chaseState = new PartyMonsterChaseState(this, animator, agent, playerDetector.Player);
        var attackState = new PartyMonsterAttackState(this, animator, agent, playerDetector.Player);
        var dieState = new PartyMonsterDieState(this, animator);
        var getHitState = new PartyMonsterGetHitState(this, animator, agent);

        At(idleState, chaseState, new FuncPredicate(() => playerDetector.CanDetectPlayer()));
        At(chaseState, idleState, new FuncPredicate(() => !playerDetector.CanDetectPlayer()));
        At(chaseState, attackState, new FuncPredicate(() => playerDetector.CanAttackPlayer()));
        At(attackState, chaseState, new FuncPredicate(() => !playerDetector.CanAttackPlayer()));

        Any(getHitState, new FuncPredicate(() => !deathTimer.IsRunning && getHitTimer.IsRunning));
        At(getHitState, attackState, new FuncPredicate(() => !getHitTimer.IsRunning && playerDetector.CanAttackPlayer()));
        At(getHitState, chaseState, new FuncPredicate(() => !getHitTimer.IsRunning && !playerDetector.CanAttackPlayer()));
        Any(dieState, new FuncPredicate(() => deathTimer.IsRunning));

        stateMachine.SetState(idleState);
    }

    protected void At(IState from, IState to, IPredicate condition) => stateMachine.AddTransition(from, to, condition);
    protected void Any(IState to, IPredicate condition) => stateMachine.AddAnyTransition(to, condition);

    protected virtual void Update()
    {
        FallDetect();
        stateMachine.Update();
        HandleTimers();

        UpdateAnimator();
    }

    protected virtual void FixedUpdate()
    {
        stateMachine.FixedUpdate();
    }

    private void UpdateAnimator()
    {
        animator.SetFloat(deathProgress, 1 - deathTimer.Progress);
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
        canAttack = false;
        Debug.Log("Attack");

        attackTimer.Start();
        playerDetector.Player.GetComponent<PlayerController>().TakeDamage(1);
    }

    public virtual void TakeDamage(float damage)
    {
        GetComponent<Health>().TakeDamage((int)damage);
        getHitTimer.Start();
        if(GetComponent<Health>().currentHealth <= 0)
        {
            Die();
        }
    }

    public void KnockAway(float attackForce)
    {
        return;

        if(GetComponent<Health>().currentHealth > 0) return;
        if(knockAwaySound != null)
            audioSource.PlayOneShot(knockAwaySound);

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

        agent.enabled = false;
        rb.isKinematic = true;

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

    // 检测玩家是否掉入虚空
    private void FallDetect()
    {
        if (transform.position.y < -10)
        {
            DeathSequence();
        }
    }
}
