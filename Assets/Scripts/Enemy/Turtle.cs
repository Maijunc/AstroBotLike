using UnityEngine;
using static Timer;

public class Turtle : Enemy
{
    [Header("Defend Settings")]
    [SerializeField] float defendCooldown = 5f; // 防御冷却时间
    [SerializeField] float defendDuration = 3f; // 防御持续时间

    [Header("Dash Settings")]
    [SerializeField] float dashDuration = 1.5f; // 冲刺持续时间
    [SerializeField] float dashSpeed = 4f; // 冲刺速度

    private CountdownTimer defendCooldownTimer; // 防御冷却计时器
    private CountdownTimer defendTimer;        // 防御状态计时器
    private CountdownTimer dashTimer;         // 冲刺计时器

    private bool isDefending; // 是否处于防御状态

    protected override void Awake()
    {
        if(playerDetector.Player == null)
        {
            // Debug.Log("Player Transform is null in Turtle.");
            playerDetector.GetPlayer();
        }

        base.Awake();

        if(playerDetector.Player == null)
        {
            Debug.Log("Player Transform is null in Turtle.");
        }

        // 初始化计时器
        defendCooldownTimer = new CountdownTimer(defendCooldown); // 冷却时间 5 秒
        defendTimer = new CountdownTimer(defendDuration);        // 防御时间 3 秒
        dashTimer = new CountdownTimer(dashDuration); // 冲刺时间 1 秒

        defendCooldownTimer.OnTimerStop += () => {
            defendTimer.Start();
        };

        defendTimer.OnTimerStop += () => {
            defendCooldownTimer.Start(); // 防御结束后启动冷却计时器
            dashTimer.Start(); // 启动冲刺计时器
        };


        timers.Add(defendCooldownTimer);
        timers.Add(defendTimer);
        timers.Add(dashTimer);

        var defendState = new TurtleDefendState(this, animator, agent);
        var wanderState = new EnemyWanderState(this, animator, agent, wanderRadius);
        var dashState = new TurleDashState(this, animator, agent, dashSpeed, playerDetector.Player);
        var chaseState = new EnemyChaseState(this, animator, agent, playerDetector.Player);
        var attackState = new EnemyAttackState(this, animator, agent, playerDetector.Player);

        Any(defendState, new FuncPredicate(() => !deathTimer.IsRunning && defendTimer.IsRunning && playerDetector.CanDetectPlayer()));
        At(defendState, dashState, new FuncPredicate(() => !defendTimer.IsRunning && dashTimer.IsRunning));

        At(dashState, attackState, new FuncPredicate(() => playerDetector.CanAttackPlayer(attackRange)));
        At(dashState, chaseState, new FuncPredicate(() => !dashTimer.IsRunning && !playerDetector.CanAttackPlayer(attackRange) && playerDetector.CanDetectPlayer()));
        At(dashState, wanderState, new FuncPredicate(() => !dashTimer.IsRunning && !playerDetector.CanAttackPlayer(attackRange) && !playerDetector.CanDetectPlayer()));
        defendCooldownTimer.Start();
    }

    protected override void Update()
    {
        base.Update();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    

    public override void TakeDamage(float damage)
    {
        if (defendTimer.IsRunning)
        {
            Debug.Log("Turtle is defending, no damage taken!");
            return;
        }

        base.TakeDamage(damage);
    }
}
