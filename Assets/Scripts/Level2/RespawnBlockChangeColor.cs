using UnityEngine;

public class RespawnBlockChangeColor : MonoBehaviour
{
    public Material newColor;  // 物体踩上时的颜色
    private Renderer platformRenderer;
    private bool ifTriggered = false;

    void Start()
    {
        // 获取物体的Renderer组件并记录初始颜色
        platformRenderer = GetComponent<Renderer>();
    }

    // 当物体与玩家发生碰撞时触发
    private void OnCollisionEnter(Collision collision)
    {
        // 检查碰撞的物体是不是玩家
        if (collision.gameObject.CompareTag("Player")&& !ifTriggered)
        {
            // 获取玩家的 PlayerController 脚本并修改属性
            PlayerController player = collision.gameObject.GetComponent<PlayerController>();
            if (player != null)
            {
                // 改变物体颜色
                platformRenderer.material = newColor;
                // 修改玩家脚本中的属性
                player.spawnPoint = transform;
                ifTriggered = true;
            }
        }
    }
}
