using System;
using System.Collections;
using Cinemachine;
using KBCore.Refs;
using UnityEngine;

public class CameraManager : MonoBehaviour 
{
    [SerializeField, Anywhere] InputReader input;
    [SerializeField, Anywhere] CinemachineFreeLook freeLookCam;

    [Header("Settings")]
    [SerializeField, Range(0.5f, 3f)] float speedMultiplier = 1f;

    // 右键是否按下，用于判断是否启用鼠标控制摄像头。
    bool isRMBPressed;
    // 设备是否为鼠标。
    bool IsDeviceMouse;
    // 摄像头移动锁定状态，用于短时间内禁止摄像头移动。
    bool cameraMovementLock;

    void OnEnable()
    {
        // 订阅事件 当input发布事件的时候就会调用这些订阅了的函数
        input.Look += OnLook;
        input.EnableMouseControlCamera += OnEnableMouseControlCamera;
        input.DisableMouseControlCamera += OnDisableMouseControlCamera;
    }
    void OnDisable()
    {
        input.Look -= OnLook;
        input.EnableMouseControlCamera -= OnEnableMouseControlCamera;
        input.DisableMouseControlCamera -= OnDisableMouseControlCamera;
    }
    private void OnLook(Vector2 cameraMovement, bool IsDeviceMouse)
    {
        if (cameraMovementLock) return; //如果锁定 直接返回

        if (IsDeviceMouse && !isRMBPressed) return; // 鼠标模式下必须按下右键
        
        // 如果设备是鼠标，那么使用固定的时间间隔，否则使用普通的时间间隔
        float devicemultiplier = IsDeviceMouse ? Time.fixedDeltaTime : Time.deltaTime;

        // Set the camera axis values
        freeLookCam.m_XAxis.m_InputAxisValue = cameraMovement.x * speedMultiplier * devicemultiplier;
        freeLookCam.m_YAxis.m_InputAxisValue = cameraMovement.y * speedMultiplier * devicemultiplier;
    }

    // 启用鼠标控制摄像头时锁定鼠标指针，并在短时间内禁止摄像头的移动操作
    private void OnEnableMouseControlCamera()
    {
        isRMBPressed = true;

        // Lock the cursor to the center of the screen and hide it
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        StartCoroutine(DisableMouseForFrame());
    }

    private void OnDisableMouseControlCamera()
    {
        isRMBPressed = false;

        // Unlock the cursor to the center of the screen and hide it
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Reset the camera axis to prevent jumping when re-enabling mouse control
        freeLookCam.m_XAxis.m_InputAxisValue = 0f;
        freeLookCam.m_YAxis.m_InputAxisValue = 0f;
    }

    private IEnumerator DisableMouseForFrame()
    {
        cameraMovementLock = true;
        yield return new WaitForEndOfFrame();
        cameraMovementLock = false;
    }
}
