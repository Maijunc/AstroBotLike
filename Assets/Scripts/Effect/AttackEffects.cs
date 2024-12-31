using System;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AttackEffects : MonoBehaviour
{
    public Transform player { get; private set; }
    // 横劈攻击特效和音效
    [Header("Horizontal Slash")]
    [SerializeField] GameObject horizontalSlashVFX;  // 横劈时显示的特效
    [SerializeField] AudioClip horizontalSlashSound; // 横劈的音效

    // 斜砍攻击特效和音效
    [Header("Diagonal Slash")]
    [SerializeField] GameObject diagonalSlashVFX;  // 斜砍时显示的特效
    [SerializeField] AudioClip diagonalSlashSound; // 斜砍的音效

    // 旋转攻击特效和音效
    [Header("Spin Attack")]
    [SerializeField] GameObject spinAttackVFX;     // 旋转攻击时显示的特效
    [SerializeField] AudioClip spinAttackSound;    // 旋转攻击的音效
    private GameObject curVFX;

    // public LineRenderer rangeLineRenderer;  // 用于绘制攻击范围的 LineRenderer


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
            // 监听 horizontalSlashCooldownTimer 事件
            playerController.horizontalSlashCooldownTimer.OnTimerStart += HandleHorizontalSlashStart;
            playerController.horizontalSlashCooldownTimer.OnTimerStop += HandleHorizontalSlashStop;

            // 监听 diagonalSlashCooldownTimer 事件
            playerController.diagonalSlashCooldownTimer.OnTimerStart += HandleDiagonalSlashStart;
            playerController.diagonalSlashCooldownTimer.OnTimerStop += HandleDiagonalSlashStop;

            // 监听 spinAttackCooldownTimer 事件
            playerController.spinAttackCooldownTimer.OnTimerStart += HandleSpinAttackStart;
            playerController.spinAttackCooldownTimer.OnTimerStop += HandleSpinAttackStop;
        }
    }

    private void HandleSpinAttackStart()
    {
        Debug.Log("旋转攻击特效开始");
        if(spinAttackVFX != null)
        {
            curVFX = Instantiate(spinAttackVFX, player.transform.position, player.transform.rotation);
            curVFX.transform.SetParent(transform);  // 将特效作为当前物体的子物体，确保特效随角色移动
        }

        // 播放旋转攻击的音效
        if(spinAttackSound != null)
            GetComponent<AudioSource>().PlayOneShot(spinAttackSound);
    }
    private void HandleSpinAttackStop()
    {
        if (curVFX != null)
        {
            Destroy(curVFX);
            GetComponent<AudioSource>().Stop();
        }
    }

    private void HandleDiagonalSlashStart()
    {
        Debug.Log("斜砍攻击特效开始");
        if(diagonalSlashVFX != null)
        {
            // 计算攻击位置和方向
            Vector3 attackPos = transform.position + transform.forward * 1f;  // 攻击起始位置，稍微前移1米

            var playerController = player.GetComponent<PlayerController>();

            // 获取斜砍的方向，假设当前状态为移动中，选择前左斜或前右斜（根据需要调整逻辑）
            Vector3 slashDirection = playerController.GetDiagonalSlashDirection();  // 根据角色朝向获取斜砍方向

            // 攻击特效的旋转
            Quaternion attackRotation = Quaternion.LookRotation(slashDirection);  // 让特效朝着斜砍的方向

            // 实例化攻击特效
            curVFX = Instantiate(diagonalSlashVFX, attackPos, attackRotation);
            curVFX.transform.SetParent(transform);  // 作为角色的子物体，随角色一起移动

            // 调整特效的缩放（压扁）
            curVFX.transform.localScale = new Vector3(1f, 1f, 1f);  // 压扁特效，可以根据需要调整
        }        

        // 播放斜砍攻击的音效
        if(diagonalSlashSound != null)
            GetComponent<AudioSource>().PlayOneShot(diagonalSlashSound);
    }
    private void HandleDiagonalSlashStop()
    {
        if (curVFX != null)
        {
            Destroy(curVFX);
            GetComponent<AudioSource>().Stop();
        }
    }

    private void HandleHorizontalSlashStart()
    {
        Debug.Log("攻击特效开始");
        if(horizontalSlashVFX != null)
        {
            // 计算攻击的方向和位置
            Vector3 attackPos = player.transform.position + player.transform.forward * 1f;  // 默认前方 1 米
            
            // 抬高特效位置：增加Y轴的偏移量
            attackPos.y += 1f;  // 抬高特效，可以根据需要调整这个数值

            // // 计算攻击的方向
            // Vector3 attackDirection = player.transform.forward;  // 默认是角色的朝向
            
            // // 攻击特效的方向和位置
            // Quaternion attackRotation = Quaternion.LookRotation(attackDirection);  // 让特效朝着攻击的方向
            curVFX = Instantiate(horizontalSlashVFX, attackPos, player.transform.rotation);
            curVFX.transform.SetParent(transform);  // 将特效作为当前物体的子物体，确保特效随角色移动

            // 调整特效的缩放（压扁）
            // curVFX.transform.localScale = new Vector3(1f, 0.5f, 1f);  // 压扁特效，Y轴缩放值小于1，调整为合适的比例
        }

        // 播放攻击音效
        if(horizontalSlashSound != null)
            GetComponent<AudioSource>().PlayOneShot(horizontalSlashSound);
    }

    private void HandleHorizontalSlashStop()
    {
        if (curVFX != null)
        {
            Destroy(curVFX);
            GetComponent<AudioSource>().Stop();
        }
    }


}
