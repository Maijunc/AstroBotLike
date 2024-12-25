using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;         // 目标物体（比如角色）
    public Vector3 offset = new Vector3(0, 5, -10);  // 相机相对于目标的偏移位置
    public float smoothSpeed = 0.125f;  // 平滑的跟随速度
    public float rotationSpeed = 5f; // 相机旋转速度
    public float zoomSpeed = 10f;    // 相机缩放速度
    public float minZoom = 5f;       // 最小缩放距离
    public float maxZoom = 15f;      // 最大缩放距离
    public float minVerticalAngle = -30f; // 相机俯仰角度的最小值
    public float maxVerticalAngle = 90f;  // 相机俯仰角度的最大值

    private float currentZoom = 10f; // 当前缩放距离
    private float currentVerticalAngle = 0f; // 当前俯仰角度

    void Update()
    {
        // 获取鼠标的输入来控制相机的旋转
        float horizontalInput = Input.GetAxis("Mouse X");
        float verticalInput = -Input.GetAxis("Mouse Y");

        // 根据鼠标移动控制相机的旋转（只考虑水平旋转和限制俯仰角度）
        transform.RotateAround(target.position, Vector3.up, horizontalInput * rotationSpeed);

        // 控制俯仰角度
        currentVerticalAngle += verticalInput * rotationSpeed;
        currentVerticalAngle = Mathf.Clamp(currentVerticalAngle, minVerticalAngle, maxVerticalAngle);

        // 根据限制的俯仰角度更新相机的旋转
        Quaternion rotation = Quaternion.Euler(currentVerticalAngle, transform.eulerAngles.y, 0);

        // 控制相机的缩放
        currentZoom -= Input.GetAxis("Mouse ScrollWheel") * zoomSpeed;
        currentZoom = Mathf.Clamp(currentZoom, minZoom, maxZoom);

        // 计算目标位置，保持相对的偏移量，并加入当前缩放值
        Vector3 desiredPosition = target.position + rotation * offset.normalized * currentZoom;

        // 使用平滑插值来更新相机的位置
        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        // 始终让相机朝向目标
        transform.LookAt(target);
    }
}
