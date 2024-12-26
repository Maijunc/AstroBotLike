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

    [Header("Jetpack Settings")]
    [SerializeField] float jetpackDuration = 0.5f;
    [SerializeField] float jetpackCooldown = 1f;
    [SerializeField] float jetpackForce = 6f;
    // [SerializeField] float jetpackGravityMultiplier = 3f;
    [SerializeField] float jetpackMaxHeight = 2f;

    // 防止浮动
    const float ZeroF = 0f;
    Transform mainCam;
    bool OutOfFuel = false;

    float currentSpeed;
    float velocity;
    float jumpVelocity;

    Vector3 movement;

    List<Timer> timers;
    CountdownTimer jumpTimer;
    CountdownTimer jumpCooldownTimer;
    CountdownTimer jetpackTimer;
    CountdownTimer jetpackCooldownTimer;

    // Animator parameters
    static readonly int Speed = Animator.StringToHash("Speed");

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

        // Setup timer
        jumpTimer = new CountdownTimer(jumpDuration);
        jumpCooldownTimer = new CountdownTimer(jumpCooldown);
        jetpackTimer = new CountdownTimer(jetpackDuration);
        jetpackCooldownTimer = new CountdownTimer(jetpackCooldown);
        timers = new List<Timer> { jumpTimer, jumpCooldownTimer, jetpackTimer, jetpackCooldownTimer };

        // 当完成跳跃的时候，开始冷却
        jumpTimer.OnTimerStop += () => jumpCooldownTimer.Start();
        // 当喷气时间用完的时候，开始进行喷气背包冷却，将OutOfFuel设置为true，只有在地面上才能充能
        jetpackTimer.OnTimerStop += () => {OutOfFuel = true; jetpackCooldownTimer.Start();};
    }
    void Start()
    {
        input.EnablePlayerActions();
    }

    void OnEnable()
    {
        input.Jump += OnJump;
    }

    void OnDisable()
    {
        input.Jump -= OnJump;
    }

    private void OnJump(bool performed)
    {
        // 如果玩家按下跳跃键并且不在跳跃冷却时间内并且在地面上
        if (performed && !jumpTimer.IsRunning && !jumpCooldownTimer.IsRunning && groundChecker.isGrounded) {
            jumpTimer.Start();
        } else if (!performed && jumpTimer.IsRunning) { // 如果玩家松开跳跃键并且在跳跃中
            jumpTimer.Stop(); // 停止跳跃
        } else if(performed && !jumpTimer.IsRunning && !OutOfFuel && !groundChecker.isGrounded) { // 如果玩家已经不在跳跃过程中且不在地面上但是又按下了跳跃键
            // 开始喷气
            jetpackTimer.Start();
        } else if (!performed && jetpackTimer.IsRunning) { // 如果玩家松开跳跃键并且在喷气中
            jetpackTimer.Stop(); // 停止喷气
        }
    }

    void Update() {
        movement = new Vector3(input.Direction.x, 0f, input.Direction.y);

        HandleTimers();

        UpdateAnimator();
    }

    void FixedUpdate()
    {
        HandleJump();
        
        HandleMovement();
    }

    private void HandleTimers()
    {
        foreach (var timer in timers)
        {
            timer.Tick(Time.deltaTime);
        }
    }

    private void HandleJump()
    {
        // If not jumping or jetpacking and grounded, keep jump velocity at 0
        if (!jumpTimer.IsRunning && !jetpackTimer.IsRunning && groundChecker.isGrounded) {
            jumpVelocity = ZeroF;
            // jumpTimer.Stop();
            // jetpackTimer.Stop();
            OutOfFuel = false;
            return;
        }

        if(OutOfFuel) {

        }

        // If jumping or falling calculate velocity
        if (jumpTimer.IsRunning) {
            // Progress point for initial burst of velocity
            float launchPoint = 0.9f;
            if (jumpTimer.Progress > launchPoint)
            {
                // Calculate velocity required to reach the jump height using physics formula v = sqrt(2gh)
                jumpVelocity = Mathf.Sqrt(2 * jumpMaxHeight * Mathf.Abs(Physics.gravity.y));
            } else {
                // Gradually apply less velocity as the jump progresses
                jumpVelocity += (1 - jumpTimer.Progress) * jumpForce * Time.deltaTime;
            }
        } else { 
            // 如果在喷气状态
            if(jetpackTimer.IsRunning) {
                // Progress point for initial burst of velocity
                float launchPoint = 0.9f;
                if (jetpackTimer.Progress > launchPoint)
                {
                    // Calculate velocity required to reach the jump height using physics formula v = sqrt(2gh)
                    jumpVelocity = Mathf.Sqrt(2 * jetpackMaxHeight * Mathf.Abs(Physics.gravity.y));
                } else {
                    // Gradually apply less velocity as the jump progresses
                    jumpVelocity += (1 - jetpackTimer.Progress) * jetpackForce * Time.deltaTime;
                }
            } else {
                // Gravity takes over 自由落体
                jumpVelocity += Physics.gravity.y * gravityMultiplier * Time.deltaTime;
            }
        }

        // Apply velocity
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpVelocity, rb.linearVelocity.z);
    }

    private void UpdateAnimator()
    {
        animator.SetFloat(Speed, currentSpeed); 
    }

    private void HandleMovement()
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
        } else {
            // 当没有输入方向时，减速至零
            SmoothSpeed(ZeroF);

            // Reset horizontal velocity for a snappy stop
            rb.linearVelocity = new Vector3(ZeroF, rb.linearVelocity.y, ZeroF);
        }
    }

    void HandleHorizontalController(Vector3 adjustedDirection)
    {
        Vector3 velocity = adjustedDirection * moveSpeed * Time.fixedDeltaTime;
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
