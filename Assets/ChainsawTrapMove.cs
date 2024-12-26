using UnityEngine;

public class ChainsawTrapMove : MonoBehaviour
{
    public float moveSpeed = 3f;    // 移动速度
    public float moveRange = 5f;    // 移动范围
    private Vector3 startPosition;  // 起始位置
    private bool movingForward = true;  // 移动方向标志

    void Start()
    {
        // 记录陷阱的初始位置
        startPosition = transform.position;
    }

    void Update()
    {
        // 计算陷阱的当前目标位置
        float moveDirection = movingForward ? 1f : -1f;
        transform.position = new Vector3(startPosition.x + moveDirection * moveRange * Mathf.PingPong(Time.time * moveSpeed, 1f), transform.position.y, transform.position.z);
    }
}
