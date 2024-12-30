using UnityEngine;

public class ChargeEffects : MonoBehaviour
{
    public Transform player { get; private set; }
    [SerializeField] private GameObject chargeVFX;  // 充能开始时显示的特效
    private GameObject curVFX;

    private void Awake()
    {
        // 获取玩家对象
        player = GameObject.FindGameObjectWithTag("Player").transform;
        
    }

    private void Start()
    {
        // 给 player 赋值 chargeTimer 并且监听其开始与停止事件
        var playerController = player.GetComponent<PlayerController>();
        if (playerController != null)
        {
            // 监听 chargeTimer 事件
            playerController.chargeTimer.OnTimerStart += HandleChargeStart;
            playerController.chargeTimer.OnTimerStop += HandleChargeStop;
        }
    }

    private void HandleChargeStart()
    {
        // 当充能开始时生成特效
        if (chargeVFX != null)
        {
            // 生成充能开始时的特效
            curVFX = Instantiate(chargeVFX, this.transform.position, Quaternion.identity);
            curVFX.transform.SetParent(transform);  // 将特效作为当前物体的子物体
        }
    }

    private void HandleChargeStop()
    {
        // 当充能停止时生成特效
        if (curVFX != null)
        {
            // 生成充能停止时的特效
            Destroy(curVFX);
        }

    }
}
