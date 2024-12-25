using System;
using Cinemachine;
using KBCore.Refs;
using UnityEngine;

// ValidatedMonoBehaviour is a custom class that validates the references of the MonoBehaviour
public class PlayerController : ValidatedMonoBehaviour 
{
    [Header("References")]
    [SerializeField, Self] CharacterController controller;
    [SerializeField, Self] Animator animator;
    [SerializeField, Anywhere] CinemachineFreeLook freeLookCam;
    [SerializeField, Anywhere] InputReader input;

    [Header("Settings")]
    [SerializeField] float moveSpeed = 6f;
    [SerializeField] float rotationSpeed = 15f;
    // 决定动画多久进行改变
    [SerializeField] float smoothTime = 0.2f;

    // 防止浮动
    const float ZeroF = 0f;
    Transform mainCam;

    float currentSpeed;
    float velocity;

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
    }
    void Start()
    {
        input.EnablePlayerActions();
    }

    void Update() {
        HandleMovement();
        UpdateAnimator();
    }

    private void UpdateAnimator()
    {
        animator.SetFloat(Speed, currentSpeed); 
    }

    private void HandleMovement()
    {
        // 获取输入方向向量
        var movementDirction = new Vector3(input.Direction.x, 0f, input.Direction.y);
        // 调整输入方向相对于摄像机的方向
        var adjustedDirection = Quaternion.AngleAxis(mainCam.eulerAngles.y, Vector3.up) * movementDirction;
        
        if (adjustedDirection.magnitude > ZeroF) 
        {
            // 旋转角色朝向移动方向
            HandleRotation(adjustedDirection);
            // 移动角色
            HandleCharacterController(adjustedDirection);
            // 平滑调整速度
            SmoothSpeed(adjustedDirection.magnitude);
        } else {
            // 当没有输入方向时，减速至零
            SmoothSpeed(ZeroF);
        }
    }

    Vector3 HandleCharacterController(Vector3 adjustedDirection)
    {
        var targetPosition = adjustedDirection * (moveSpeed * Time.deltaTime);
        controller.Move(targetPosition);
        return targetPosition;
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
