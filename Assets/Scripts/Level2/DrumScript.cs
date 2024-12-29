using DG.Tweening;
using UnityEngine;

public class DrumScript : MonoBehaviour
{
    public float rotationSpeed = 90f;// 旋转速度，单位：度/秒
    public float rotationX = 90f;  
    public float rotationY = 90f;
    public float rotationZ = 90f;

    void Start()
    {
        // 持续绕 Y 轴旋转
        RotateObject();
    }
    void RotateObject()
    {
        // 旋转一圈需要的时间 = 360 / 旋转速度（度每秒）
        float rotationDuration = 360f / rotationSpeed;

        // 使用 DoTween 来持续旋转
        transform.DORotate(new Vector3(rotationX, rotationY, rotationZ), rotationDuration, RotateMode.FastBeyond360)
                 .SetLoops(-1, LoopType.Incremental);
    }
}
