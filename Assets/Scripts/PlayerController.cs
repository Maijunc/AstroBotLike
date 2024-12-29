using System;
using System.Collections.Generic;
using Cinemachine;
using KBCore.Refs;
using UnityEngine;
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
    [SerializeField, Anywhere] Transform spawnPoint;

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

    [Header("Physics Materials")]
    [SerializeField] PhysicsMaterial noFriction;
    [SerializeField] PhysicsMaterial haveFriction;

    [Header("Attack Settings")]
    // attackCoodown
    [SerializeField] float attackCooldown = 0.5f;
    [SerializeField] float attackDistance = 2f;
    [SerializeField] float attackDamage = 10f;

    // 防止浮动
    const float ZeroF = 0f;
    Transform mainCam;
    bool canUseLaserJump = false;

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
    CountdownTimer attackCooldownTimer;

    StateMachine stateMachine;

    // Animator parameters
    static readonly int Speed = Animator.StringToHash("Speed");
    static readonly int yVelocity = Animator.StringToHash("yVelocity");

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

        // 当完成跳跃的时候，开始冷却
        jumpTimer.OnTimerStop += () => jumpCooldownTimer.Start();
        // 当喷气时间用完的时候，开始进行喷气背包冷却，将OutOfFuel设置为true，只有在地面上才能充能
        laserTimer.OnTimerStop += () => {
            canUseLaserJump = false;
            laserCooldownTimer.Start();
        };
        dashTimer.OnTimerStart += () => {
            Debug.Log("dash");
            dashVelocity = dashForce;
        };
        dashTimer.OnTimerStop += () => {
            dashVelocity = 1f;
            dashCooldownTimer.Start();
        };

        // State Machine
        stateMachine = new StateMachine();

        // Declare states
        var LocomotionState = new LocomotionState(this, animator);
        var JumpState = new JumpState(this, animator);
        var LaserJumpState = new LaserJumpState(this, animator);
        var DashState = new DashState(this, animator);
        var AttackState = new AttackState(this, animator);

        // 人物的是否运动的判断来自于状态机
        // Define transitions
        // 如果 jumpTimer 还在运行，那么就从 LocomotionState 转换到 JumpState 人物正在跳跃

        At(LocomotionState, JumpState, new FuncPredicate(() => jumpTimer.IsRunning));
        // 人物在地面上，且不在跳跃状态，那么就从 JumpState 转换到 LocomotionState 表示落地了
        At(JumpState, LocomotionState, new FuncPredicate(() => groundChecker.isGrounded && !jumpTimer.IsRunning && !dashTimer.IsRunning));
        At(JumpState, LaserJumpState, new FuncPredicate(() => laserTimer.IsRunning));
        At(LaserJumpState, JumpState, new FuncPredicate(() => !laserTimer.IsRunning));

        Any(DashState, new FuncPredicate(() => dashTimer.IsRunning));
        // At(DashState, JumpState, new FuncPredicate(() => !groundChecker.isGrounded && !dashTimer.IsRunning && jumpTimer.IsRunning));
        At(DashState, LocomotionState, new FuncPredicate(() => !dashTimer.IsRunning));

        At(LocomotionState, AttackState, new FuncPredicate(() => attackCooldownTimer.IsRunning));
        At(AttackState, LocomotionState, new FuncPredicate(() => !attackCooldownTimer.IsRunning));
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

    void OnAttack()
    {
        if (!attackCooldownTimer.IsRunning)
        {
            attackCooldownTimer.Start();
        }
    }

    public void Attack()
    {
        Vector3 attackPos = transform.position + transform.forward;
        Collider[] hitEnemies = Physics.OverlapSphere(attackPos, attackDistance);

        foreach (var enemy in hitEnemies)
        {
            Debug.Log(enemy.name);
            if (enemy.CompareTag("Enemy"))
            {
                enemy.GetComponent<Health>().TakeDamage((int)attackDamage);
                enemy.GetComponent<Enemy>().Die();
            }
        }
    }

    public void Die(float playerHealthPercentage)
    {
        if (playerHealthPercentage <= 0)
        {
            // Debug.Log(playerHealthPercentage);
            this.GetComponent<Health>().ResetHP();
            rb.linearVelocity = Vector3.zero;
            this.transform.position = spawnPoint.position;
        }
    }

    private void OnJump(bool performed)
    {
        // 如果玩家按下跳跃键并且不在跳跃冷却时间内并且在地面上
        if (performed && !jumpTimer.IsRunning && !jumpCooldownTimer.IsRunning && groundChecker.isGrounded && !attackCooldownTimer.IsRunning)
        {
            jumpTimer.Start();
            playerCollider.material = noFriction;//材质修改 以防止跳跃中与墙壁有摩擦
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

        DropDetect();
    }

    // 检测玩家是否掉入虚空
    private void DropDetect()
    {
        if (transform.position.y < -10)
        {
            this.GetComponent<Health>().TakeDamage(100);
            Die(0);
        }
    }

    void FixedUpdate()
    {
        stateMachine.FixedUpdate();
    }

    private void SetupTimers()
    {
        // Setup timer
        jumpTimer = new CountdownTimer(jumpDuration);
        jumpCooldownTimer = new CountdownTimer(jumpCooldown);

        // Setup laser timer
        laserTimer = new CountdownTimer(laserDuration);
        laserCooldownTimer = new CountdownTimer(laserCooldown);

        // Setup dash timer
        dashTimer = new CountdownTimer(dashDuration);
        dashCooldownTimer = new CountdownTimer(dashCooldown);

        attackCooldownTimer = new CountdownTimer(attackCooldown);

        timers = new List<Timer> { jumpTimer, jumpCooldownTimer, laserTimer, laserCooldownTimer, dashTimer, dashCooldownTimer, attackCooldownTimer };
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
            jumpVelocity = ZeroF;
            playerCollider.material = haveFriction;
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
        animator.SetFloat(Speed, currentSpeed);
        animator.SetFloat(yVelocity, rb.linearVelocity.y);
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