using UnityEngine;

public class GroundChecker : MonoBehaviour 
{
    [SerializeField] float groundDistance = 0.08f;
    [SerializeField] LayerMask groundLayers;

    public bool isGrounded { get; private set; } 
    private bool wasGrounded = false; // 缓冲状态

    void Update()
    {
        // 地面检测
        bool currentlyGrounded = Physics.SphereCast(transform.position, groundDistance, Vector3.down, out _, groundDistance, groundLayers);
        
        // 平滑地面状态
        isGrounded = currentlyGrounded || wasGrounded;
        wasGrounded = currentlyGrounded;

    }

}
