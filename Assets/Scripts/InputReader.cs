using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "InputReader", menuName = "Input/InputReader")]
public class InputReader : ScriptableObject, PlayerInputActions.IPlayerActions
{
    // 当玩家输入方向键或控制摇杆时，发布 Move 事件，将玩家的移动方向作为参数传递给订阅者。
    public event UnityAction<Vector2> Move = delegate {};
    public event UnityAction<Vector2, bool> Look = delegate {};
    public event UnityAction EnableMouseControlCamera = delegate {};
    public event UnityAction DisableMouseControlCamera = delegate {};
    
    PlayerInputActions inputActions;

    public Vector3 Direction => inputActions.Player.Move.ReadValue<Vector2>();
    
    void OnEnable() {
        if (inputActions == null) 
        {
            inputActions = new PlayerInputActions();
            inputActions.Player.SetCallbacks(this);
        }
    }

    public void EnablePlayerActions() 
    {
        inputActions.Enable();
    }

    public void OnFire(InputAction.CallbackContext context)
    {
        // noop
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        // noop
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        Look.Invoke(context.ReadValue<Vector2>(), IsDeviceMouse(context));
    }

    private bool IsDeviceMouse(InputAction.CallbackContext context) => context.control.device.name == "Mouse";

    public void OnMouseControlCamera(InputAction.CallbackContext context)
    {
        switch (context.phase)
        {
            case InputActionPhase.Started:
                EnableMouseControlCamera.Invoke();
                break;
            case InputActionPhase.Canceled:
                DisableMouseControlCamera.Invoke();
                break;
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        Move.Invoke(context.ReadValue<Vector2>());
    }

    public void OnRun(InputAction.CallbackContext context)
    {
        // noop
    }
}
