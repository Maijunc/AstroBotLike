using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using KBCore.Refs;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using static Timer;
public class PlayerController : ValidatedMonoBehaviour
{
    [Header("References")]
    [SerializeField, Self] Rigidbody rb;
    [SerializeField, Self] Animator animator;
    [SerializeField, Anywhere] CinemachineFreeLook freeLookCam;
    [SerializeField, Anywhere] InputReader input;
    [SerializeField, Anywhere] GroundChecker groundChecker;
    [SerializeField, Anywhere] Collider playerCollider;
    [SerializeField, Anywhere] public Transform spawnPoint;

    [Header("Settings")]
    [SerializeField] float moveSpeed = 6f;
    [SerializeField] float rotationSpeed = 15f;
    // 决定动画多久进行改变
    [SerializeField] float smoothTime = 0.2f;

    [Header("Jump Settings")]
    [SerializeField] float jumpForce = 10f;
    [SerializeField] float jumpDuration = 0.5f;
    [SerializeField] float jumpCooldown = 0f;
    [SerializeField] float jumpMaxHeight = 2f;
    [SerializeField] float gravityMultiplier = 3f;

    [Header("Laser Settings")]
    [SerializeField] float laserDuration = 0.5f;
    [SerializeField] float laserCooldown = 1f;
    [SerializeField] float laserForce = 6f;
    [SerializeField] float laserMaxHeight = 2f;

    [Header("Dash Settings")]
    [SerializeField] float dashForce = 10f;
    [SerializeField] float dashDuration = 1f;
    [SerializeField] float dashCooldown = 2f;

    // HorizontalSlash 横劈
    [Header("Horizontal Slash Settings")]
    [SerializeField] float horizontalSlashCooldown = 0.5f;
    [SerializeField] float horizontalSlashDistance = 2f;
    [SerializeField] float horizontalSlashDamage = 10f;

    // DiagonalSlash 斜砍
    [Header("Diagonal Slash Settings")]
    [SerializeField] float diagonalSlashCooldown = 0.5f;
    [SerializeField] float diagonalSlashDistance = 2f;
    [SerializeField] float diagonalSlashDamage = 10f;

    // 蓄力设置
    [Header("Charge Settings")]
    [SerializeField] float maxChargeTime = 2f; // 最大蓄力时间
    [SerializeField] float chargeCooldown = 0f; // 蓄力冷却时间
    [SerializeField] float chargeTimeThreshold = 0.5f; // 蓄力时间的阈值。

    // SpinAttack 旋转攻击
    [Header("Spin Attack Settings")]
    [SerializeField] float spinAttackRange = 2f;
    [SerializeField] float spinAttackDamage = 10f;
    [SerializeField] float spinAttackConeAngle = 360f; //旋转角度

    [Header("Death Settings")]
    [SerializeField] float deathTime = 2f; //死亡重置时间
    [SerializeField] float KnockForce = 20f; //怪物死亡的冲击力

    // 防止浮动
    const float ZeroF = 0f;
    Transform mainCam;
    bool canUseLaserJump = false;
    public bool IsMoving => movement.magnitude > 0;
    float currentSpeed;
    float velocity;
    float jumpVelocity;
    float laserVelocity;
    float dashVelocity = 1f;

    Vector3 movement;

    List<Timer> timers;
    CountdownTimer jumpTimer;
    CountdownTimer jumpCooldownTimer;

    CountdownTimer laserTimer;
    CountdownTimer laserCooldownTimer;

    CountdownTimer dashTimer;
    CountdownTimer dashCooldownTimer;

    public CountdownTimer horizontalSlashCooldownTimer;
    public CountdownTimer diagonalSlashCooldownTimer;
    public CountdownTimer spinAttackCooldownTimer;

    public CountdownTimer chargeTimer;
    CountdownTimer chargeCooldownTimer;

    CountdownTimer deathTimer;

    StateMachine stateMachine;

    // Animator parameters
    static readonly int speed = Animator.StringToHash("speed");
    static readonly int yVelocity = Animator.StringToHash("yVelocity");
    static readonly int spinProgress = Animator.StringToHash("spinProgress");
    static readonly int deathProgress = Animator.StringToHash("deathProgress");

    bool canHorizontalSlash = false;
    bool canDiagonalSlash = false;

    void Awake()
    {
        mainCam = Camera.main.transform;
        freeLookCam.Follow = transform;
        freeLookCam.LookAt = transform;
        // 如果角色在游戏过程中因为某种原因被突然移动 相机就会直接跳到它上面
        freeLookCam.OnTargetObjectWarped(
            transform,
            // 修正相对位置
            transform.position - freeLookCam.transform.position - Vector3.forward
        );

        rb.freezeRotation = true;

        SetupTimers();

        SetupStateMachine();
    }

    public void HandleCharge()
    {
        // 实现蓄力的逻辑
    }

    private void SetupTimers()
    {
        // Setup timers
        jumpTimer = new CountdownTimer(jumpDuration);
        jumpCooldownTimer = new CountdownTimer(jumpCooldown);
        laserTimer = new CountdownTimer(laserDuration);
        laserCooldownTimer = new CountdownTimer(laserCooldown);
        dashTimer = new CountdownTimer(dashDuration);
        dashCooldownTimer = new CountdownTimer(dashCooldown);
        spinAttackCooldownTimer = new CountdownTimer(maxChargeTime);
        diagonalSlashCooldownTimer = new CountdownTimer(diagonalSlashCooldown);
        horizontalSlashCooldownTimer = new CountdownTimer(horizontalSlashCooldown);
        deathTimer = new CountdownTimer(deathTime);
        
        // Setup charge attack timers
        chargeTimer = new CountdownTimer(maxChargeTime);
        chargeCooldownTimer = new CountdownTimer(chargeCooldown);

        timers = new List<Timer> { jumpTimer, jumpCooldownTimer, laserTimer, laserCooldownTimer, dashTimer, dashCooldownTimer, spinAttackCooldownTimer, diagonalSlashCooldownTimer, horizontalSlashCooldownTimer, chargeTimer, chargeCooldownTimer, deathTimer };

        // 当完成跳跃的时候，开始冷却
        jumpTimer.OnTimerStop += () => jumpCooldownTimer.Start();
        // 当喷气时间用完的时候，开始进行喷气背包冷却，将OutOfFuel设置为true，只有在地面上才能充能
        laserTimer.OnTimerStop += () => {
            canUseLaserJump = false;
            laserCooldownTimer.Start();
        };
        dashTimer.OnTimerStart += () => {
            dashVelocity = dashForce;
        };
        dashTimer.OnTimerStop += () => {
            dashVelocity = 1f;
            dashCooldownTimer.Start();
        };

        chargeTimer.OnTimerStop += () => {
            chargeCooldownTimer.Start();
            StartAttackTimer();
        };

        deathTimer.OnTimerStop += () => {
            DeathSequence();
        };

        horizontalSlashCooldownTimer.OnTimerStart += () => {
            canHorizontalSlash = true;
        };

        diagonalSlashCooldownTimer.OnTimerStart += () => {
            canDiagonalSlash = true;
        };
    }

    void StartAttackTimer()
    {
        if (deathTimer.IsRunning) return;
        // 进入攻击过程
        // Debug.Log("chargeTimer.Progress = " + chargeTimer.Progress);
        if (chargeTimer.Progress <= chargeTimeThreshold && !spinAttackCooldownTimer.IsRunning)
        {
            // Debug.Log("StartspinAttack");
            spinAttackCooldownTimer.Start();
        } else if(IsMoving && !diagonalSlashCooldownTimer.IsRunning)
        {   
            // Debug.Log("StartdiagonalSlash");
            diagonalSlashCooldownTimer.Start();
        } else if(!IsMoving && !horizontalSlashCooldownTimer.IsRunning)
        {
            // Debug.Log("StarthorizontalSlash");
            horizontalSlashCooldownTimer.Start();
        }
    }

    private void SetupStateMachine()
    {
        // State Machine
        stateMachine = new StateMachine();

        // Declare states
        var LocomotionState = new LocomotionState(this, animator);
        var JumpState = new JumpState(this, animator);
        var LaserJumpState = new LaserJumpState(this, animator);
        var DashState = new DashState(this, animator);
        var ChargeState = new ChargeState(this, animator);
        var HorizontalSlashState = new HorizontalSlashState(this, animator);
        var DiagonalSlashState = new DiagonalSlashState(this, animator);
        var SpinAttackState = new SpinAttackState(this, animator);
        var DeathState = new DeathState(this, animator);
        var JumpChargeState = new JumpChargeState(this, animator);

        var JumpHorizontalSlashState = new JumpHorizontalSlashState(this, animator);
        var JumpDiagonalSlashState = new JumpDiagonalSlashState(this, animator);
        var JumpSpinAttackState = new JumpSpinAttackState(this, animator);

        // 人物的是否运动的判断来自于状态机
        // Define transitions
        // 如果 jumpTimer 还在运行，那么就从 LocomotionState 转换到 JumpState 人物正在跳跃

        At(LocomotionState, JumpState, new FuncPredicate(() => jumpTimer.IsRunning));
        // 人物在地面上，且不在跳跃状态，那么就从 JumpState 转换到 LocomotionState 表示落地了
        At(JumpState, LocomotionState, new FuncPredicate(() => groundChecker.isGrounded && !jumpTimer.IsRunning && !dashTimer.IsRunning));
        At(JumpState, LaserJumpState, new FuncPredicate(() => laserTimer.IsRunning));

        // 跳跃蓄力功能
        At(JumpState, JumpChargeState, new FuncPredicate(() => chargeTimer.IsRunning && !spinAttackCooldownTimer.IsRunning && !diagonalSlashCooldownTimer.IsRunning && !horizontalSlashCooldownTimer.IsRunning));
        At(JumpChargeState, LocomotionState, new FuncPredicate(() => groundChecker.isGrounded && !jumpTimer.IsRunning && !dashTimer.IsRunning));
        At(JumpChargeState, JumpHorizontalSlashState, new FuncPredicate(() => horizontalSlashCooldownTimer.IsRunning && chargeTimer.Progress > chargeTimeThreshold && !IsMoving));
        At(JumpChargeState, JumpDiagonalSlashState, new FuncPredicate(() => diagonalSlashCooldownTimer.IsRunning && chargeTimer.Progress > chargeTimeThreshold && IsMoving));
        At(JumpChargeState, JumpSpinAttackState, new FuncPredicate(() => spinAttackCooldownTimer.IsRunning && chargeTimer.Progress <= chargeTimeThreshold));

        At(JumpHorizontalSlashState, JumpState, new FuncPredicate(() => !horizontalSlashCooldownTimer.IsRunning));
        At(JumpDiagonalSlashState, JumpState, new FuncPredicate(() => !diagonalSlashCooldownTimer.IsRunning));
        At(JumpSpinAttackState, JumpState, new FuncPredicate(() => !spinAttackCooldownTimer.IsRunning));

        // 从LaserJumpState 直接转到 LocomotionState
        At(LaserJumpState, LocomotionState, new FuncPredicate(() => !laserTimer.IsRunning && groundChecker.isGrounded));

        Any(DashState, new FuncPredicate(() => dashTimer.IsRunning));
        // At(DashState, JumpState, new FuncPredicate(() => !groundChecker.isGrounded && !dashTimer.IsRunning && jumpTimer.IsRunning));
        At(DashState, LocomotionState, new FuncPredicate(() => !dashTimer.IsRunning));

        // 移动蓄力攻击
        At(LocomotionState, ChargeState, new FuncPredicate(() => chargeTimer.IsRunning && !spinAttackCooldownTimer.IsRunning && !diagonalSlashCooldownTimer.IsRunning && !horizontalSlashCooldownTimer.IsRunning));
        At(ChargeState, HorizontalSlashState, new FuncPredicate(() => horizontalSlashCooldownTimer.IsRunning && chargeTimer.Progress > chargeTimeThreshold && !IsMoving));
        At(ChargeState, DiagonalSlashState, new FuncPredicate(() => diagonalSlashCooldownTimer.IsRunning && chargeTimer.Progress > chargeTimeThreshold && IsMoving));
        At(ChargeState, SpinAttackState, new FuncPredicate(() => spinAttackCooldownTimer.IsRunning && chargeTimer.Progress <= chargeTimeThreshold));

        // 攻击持续时间结束
        At(HorizontalSlashState, LocomotionState, new FuncPredicate(() => !horizontalSlashCooldownTimer.IsRunning));
        At(DiagonalSlashState, LocomotionState, new FuncPredicate(() => !diagonalSlashCooldownTimer.IsRunning));
        At(SpinAttackState, LocomotionState, new FuncPredicate(() => !spinAttackCooldownTimer.IsRunning));

        Any(DeathState, new FuncPredicate(() => deathTimer.IsRunning));
        At(DeathState, LocomotionState, new FuncPredicate(() => !deathTimer.IsRunning));

        At(ChargeState, JumpChargeState, new FuncPredicate(() => jumpTimer.IsRunning));
        At(HorizontalSlashState, JumpHorizontalSlashState, new FuncPredicate(() => jumpTimer.IsRunning));
        At(DiagonalSlashState, JumpDiagonalSlashState, new FuncPredicate(() => jumpTimer.IsRunning));
        At(SpinAttackState, JumpSpinAttackState, new FuncPredicate(() => jumpTimer.IsRunning));

        // Set initial state
        stateMachine.SetState(LocomotionState);
    }

    // 辅助方法 用于添加状态转换
    void At(IState from, IState to, IPredicate condition) => stateMachine.AddTransition(from, to, condition);
    void Any(IState to, IPredicate condition) => stateMachine.AddAnyTransition(to, condition);

    void Start()
    {
        input.EnablePlayerActions();
    }

    void OnEnable()
    {
        input.Jump += OnJump;
        input.Dash += OnDash;
        input.Attack += OnAttack;
    }

    void OnDisable()
    {
        input.Jump -= OnJump;
        input.Dash -= OnDash;
        input.Attack -= OnAttack;
    }

    void OnAttack(bool performed)
    {
        // 按下攻击键 开始蓄力
        if(performed && !chargeCooldownTimer.IsRunning && !spinAttackCooldownTimer.IsRunning && !laserTimer.IsRunning)
        {
            chargeTimer.Start();
        } else if(!performed) //松开攻击键的时候停止计时
        {   
            chargeTimer.Stop();
        }
    }

    // 横劈
    public void HandleHorizontalSlash() 
    { 
        if(!canHorizontalSlash) return;

        canHorizontalSlash = false;

        // 攻击位置和方向
        Vector3 attackPos = transform.position + transform.forward;
        Vector3 attackDir = transform.forward;

        Collider[] hitEnemies = Physics.OverlapSphere(attackPos, horizontalSlashDistance);

        // 锥形范围的角度限制
        float coneAngle = 45f; // 45度的锥形范围

        foreach (var enemy in hitEnemies)
        {
            // 计算目标与攻击方向的夹角
            Vector3 toEnemy = (enemy.transform.position - transform.position).normalized;
            float angleToEnemy = Vector3.Angle(attackDir, toEnemy);

            // 如果目标在锥形范围内
            if (angleToEnemy <= coneAngle && enemy.CompareTag("Enemy"))
            {
                Debug.Log($"Hit: {enemy.name}");
                enemy.GetComponent<Enemy>().TakeDamage((int)horizontalSlashDamage);
                enemy.GetComponent<Enemy>().KnockAway(KnockForce); // 击飞敌人
            }
        }
    }
    public void HandleDiagonalSlash() 
    {
        if(!canDiagonalSlash) return;
        canDiagonalSlash = false;

        // 计算攻击位置和方向
        Vector3 attackPos = transform.position + transform.forward; // 攻击起始位置
        Vector3 attackDir = transform.forward; // 默认斜砍的攻击方向是角色的朝前方向

        // 斜砍范围和角度设定
        float coneAngle = 45f; // 锥形范围的角度，假设是45度的斜砍范围

        // 获取斜砍的方向（这里可以根据角色当前朝向来判断方向是前左斜还是前右斜）
        Vector3 slashDirection = GetDiagonalSlashDirection();

        // 获取所有被攻击的敌人
        Collider[] hitEnemies = Physics.OverlapSphere(attackPos, diagonalSlashDistance);

        foreach (var enemy in hitEnemies)
        {
            // 计算目标与斜砍方向的夹角
            Vector3 toEnemy = (enemy.transform.position - transform.position).normalized;
            float angleToEnemy = Vector3.Angle(slashDirection, toEnemy);

            // 判断目标是否在攻击范围内（即夹角是否小于斜砍的角度）
            if (angleToEnemy <= coneAngle && enemy.CompareTag("Enemy"))
            {
                Debug.Log($"Hit: {enemy.name}");
                // 对敌人造成伤害
                enemy.GetComponent<Enemy>().TakeDamage((int)diagonalSlashDamage);
                enemy.GetComponent<Enemy>().KnockAway(KnockForce); // 击飞敌人
            }
        }
    }

    // 获取斜砍方向（前左斜或前右斜）
    public Vector3 GetDiagonalSlashDirection()
    {
        // 获取角色的右侧和前方方向
        Vector3 forward = transform.forward;
        Vector3 right = transform.right;

        // 右上向左下方向是前进方向 + 向左的右侧
        return (forward - right).normalized; // 斜砍方向为右上向左下
    }
    public void HandleSpinAttack() 
    {
        // 获取攻击位置和攻击方向
        Vector3 attackPos = transform.position; // 角色位置
        Vector3 attackDir = transform.forward;  // 攻击方向为角色前方的方向

        // 获取所有被攻击的敌人
        Collider[] hitEnemies = Physics.OverlapSphere(attackPos, spinAttackRange);

        // 对每个敌人进行检查
        foreach (var enemy in hitEnemies)
        {
            // 判断敌人是否在攻击范围内
            Vector3 toEnemy = (enemy.transform.position - transform.position).normalized;
            float angleToEnemy = Vector3.Angle(attackDir, toEnemy); // 计算敌人与攻击方向的夹角

            // 如果敌人在攻击范围内并且是敌人
            if (angleToEnemy <= spinAttackConeAngle && enemy.CompareTag("Enemy"))
            {
                // 输出日志并对敌人造成伤害
                Debug.Log($"Hit: {enemy.name}");
                enemy.GetComponent<Enemy>().TakeDamage((int)spinAttackDamage); // 对敌人造成伤害
                enemy.GetComponent<Enemy>().KnockAway(KnockForce); // 击飞敌人
            }
        }
    }

    public void TakeDamage(float damage)
    {
        GetComponent<Health>().TakeDamage((int)damage);
        if(!deathTimer.IsRunning && GetComponent<Health>().currentHealth <= 0)
            Die();
    }

    private void Die()
    {
        rb.linearVelocity = Vector3.zero; // 停止当前的速度
        rb.angularVelocity = Vector3.zero;     // 停止当前的角速度
        // 设置为 kinematic，意味着物体不再受物理引擎控制，但仍然能够响应脚本控制
        rb.isKinematic = true;

        deathTimer.Start();
    }

    // 死亡后流程
    private void DeathSequence()
    {
        this.GetComponent<Health>().ResetHP();
        
        this.transform.position = spawnPoint.position;
        // 将其重置为初始状态
        this.transform.rotation = Quaternion.identity;

        // 重置摄像头位置
        freeLookCam.OnTargetObjectWarped(
            transform,
            // 修正相对位置
            transform.position - freeLookCam.transform.position - Vector3.forward
        );

        // // 重置场景
        // ResetScene();

        // 重置场景
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    void ResetScene()
    {
        // 获取当前场景的名称
        string currentSceneName = SceneManager.GetActiveScene().name;

        // 重载当前场景
        SceneManager.LoadScene(currentSceneName);
    }

    private void OnJump(bool performed)
    {
        // 如果玩家按下跳跃键并且不在跳跃冷却时间内并且在地面上
        if (performed && !jumpTimer.IsRunning && !jumpCooldownTimer.IsRunning && groundChecker.isGrounded)
        {
            jumpTimer.Start();
        }
        else if (!performed && jumpTimer.IsRunning)
        { // 如果玩家松开跳跃键并且在跳跃中
            jumpTimer.Stop(); // 停止跳跃
        }
        else if (performed && !jumpTimer.IsRunning && canUseLaserJump && !groundChecker.isGrounded)
        { // 如果玩家已经不在跳跃过程中且不在地面上但是又按下了跳跃键
            // 开始喷气
            laserTimer.Start();
        }
        else if (!performed && laserTimer.IsRunning)
        { // 如果玩家松开跳跃键并且在喷气中
            laserTimer.Stop(); // 停止喷气
        }
    }

    private void OnDash(bool performed)
    {
        if (performed && !dashTimer.IsRunning && !dashCooldownTimer.IsRunning)
        {
            dashTimer.Start();
        }
        else if (!performed && dashTimer.IsRunning)
        {
            dashTimer.Stop();
        }
    }

    void Update()
    {
        movement = new Vector3(input.Direction.x, 0f, input.Direction.y);
        stateMachine.Update();

        HandleTimers();

        UpdateAnimator();

        FallDetect();
    }

    // 检测玩家是否掉入虚空
    private void FallDetect()
    {
        if (transform.position.y < -10)
        {
            DeathSequence();
        }
    }

    void FixedUpdate()
    {
        stateMachine.FixedUpdate();
    }

    public void HandleLaserJump()
    {
        // 如果在喷气状态
        if (laserTimer.IsRunning)
        {
            // Progress point for initial burst of velocity
            float launchPoint = 0.9f;
            if (laserTimer.Progress > launchPoint)
            {
                // Calculate velocity required to reach the jump height using physics formula v = sqrt(2gh)
                laserVelocity = Mathf.Sqrt(2 * laserMaxHeight * Mathf.Abs(Physics.gravity.y));
            }
            else
            {
                // Gradually apply less velocity as the jump progresses
                laserVelocity += (1 - laserTimer.Progress) * laserForce * Time.deltaTime;
            }
        }
        else
        {
            // Gravity takes over 自由落体
            laserVelocity += Physics.gravity.y * gravityMultiplier * Time.deltaTime;
        }

        // Apply velocity
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, laserVelocity, rb.linearVelocity.z);
    }

    private void HandleTimers()
    {
        foreach (var timer in timers)
        {
            timer.Tick(Time.deltaTime);
        }
    }

    public void HandleJump()
    {
        // If not jumping or lasering and grounded, keep jump velocity at 0 落地了
        if (!jumpTimer.IsRunning && !laserTimer.IsRunning && groundChecker.isGrounded)
        {
            jumpVelocity = 0;
            canUseLaserJump = false;
            return;
        }

        if (!canUseLaserJump && jumpTimer.IsRunning && jumpTimer.Progress > 0.5f)
        {
            canUseLaserJump = true;
        }

        // If jumping or falling calculate velocity
        if (jumpTimer.IsRunning)
        {
            // Progress point for initial burst of velocity
            float launchPoint = 0.9f;
            if (jumpTimer.Progress > launchPoint)
            {
                // Calculate velocity required to reach the jump height using physics formula v = sqrt(2gh)
                jumpVelocity = Mathf.Sqrt(2 * jumpMaxHeight * Mathf.Abs(Physics.gravity.y));
            }
            else
            {
                // Gradually apply less velocity as the jump progresses
                jumpVelocity += (1 - jumpTimer.Progress) * jumpForce * Time.deltaTime;
            }
        }
        else
        {
            // Gravity takes over 自由落体
            jumpVelocity += Physics.gravity.y * gravityMultiplier * Time.deltaTime;
        }

        // Apply velocity
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpVelocity, rb.linearVelocity.z);
    }

    private void UpdateAnimator()
    {
        animator.SetFloat(speed, currentSpeed);
        animator.SetFloat(yVelocity, rb.linearVelocity.y);
        animator.SetFloat(spinProgress, 1 - spinAttackCooldownTimer.Progress);
        animator.SetFloat(deathProgress, 1 - deathTimer.Progress);
    }

    public void HandleMovement()
    {
        // 调整输入方向相对于摄像机的方向
        var adjustedDirection = Quaternion.AngleAxis(mainCam.eulerAngles.y, Vector3.up) * movement;

        if (adjustedDirection.magnitude > ZeroF)
        {
            // 旋转角色朝向移动方向
            HandleRotation(adjustedDirection);
            // 移动角色
            HandleHorizontalController(adjustedDirection);
            // 平滑调整速度
            SmoothSpeed(adjustedDirection.magnitude);
        }
        else
        {
            // 当没有输入方向时，减速至零
            SmoothSpeed(ZeroF);

            // Reset horizontal velocity for a snappy stop
            rb.linearVelocity = new Vector3(ZeroF, rb.linearVelocity.y, ZeroF);
        }
    }

    void HandleHorizontalController(Vector3 adjustedDirection)
    {
        Vector3 velocity = adjustedDirection * (moveSpeed * dashVelocity * Time.fixedDeltaTime);
        rb.linearVelocity = new Vector3(velocity.x, rb.linearVelocity.y, velocity.z);
    }
    void HandleRotation(Vector3 adjustedDirection)
    {
        var targetRotation = Quaternion.LookRotation(adjustedDirection);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        transform.LookAt(transform.position + adjustedDirection);
    }

    void SmoothSpeed(float value)
    {
        currentSpeed = Mathf.SmoothDamp(currentSpeed, value, ref velocity, smoothTime);
    }
}