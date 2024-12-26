using UnityEngine;
using DG.Tweening;

public class FilpTrapScript : MonoBehaviour
{
    public Transform stickObject; // 棍状物体
    public Transform panelLeft;   // 左侧板状物体
    public Transform panelRight;  // 右侧板状物体

    public float rotationAngle = 90f;  // 每次旋转的角度（例如 90 度）
    public float rotationDuration = 2f; // 一次旋转的时间
    public float delayBeforeStart = 0f; // 延迟开始的时间

    void Start()
    {
        // 启动板状物体围绕棍状物体旋转
        RotatePanel(panelLeft, stickObject.position);
        RotatePanel(panelRight, stickObject.position);
    }

    void RotatePanel(Transform panel, Vector3 center)
    {
        // 计算板状物体和棍状物体之间的偏移量（即相对位置）
        Vector3 offset = panel.position - center;

        // 设置旋转方向和角度（这里设置绕 Z 轴旋转）
        panel.DORotate(new Vector3(0, 0, rotationAngle), rotationDuration, RotateMode.LocalAxisAdd)
            .SetDelay(delayBeforeStart)  // 延迟开始旋转
            .SetLoops(-1, LoopType.Yoyo)  // 无限循环旋转，并且来回旋转
            .SetEase(Ease.Linear);  // 使用线性过渡
    }
}
