using UnityEngine;
using DG.Tweening;

public class FilpTrapScript : MonoBehaviour
{
    public float rotateAngle = 90f;  // 旋转的角度
    public float rotateDuration = 2f;  // 每次旋转的时间
    public float waitTime = 1f;  // 每次旋转后的等待时间

    private Sequence rotationSequence;

    private void Start()
    {
        // 开始旋转循环
        StartRotating();
    }

    void Update()
    {
        // 在每帧检查对象是否已销毁
        if (transform == null && rotationSequence != null)
        {
            rotationSequence.Kill();  // 停止和销毁该Tween
        }
    }

    void OnDestroy()
    {
        // 确保当对象销毁时，取消所有相关Tween
        if (rotationSequence != null)
        {
            DOTween.KillAll();
        }
    }

    private void StartRotating()
    {
        // 创建一个Sequence
        rotationSequence = DOTween.Sequence();

        // 顺时针旋转到指定角度
        rotationSequence.Append(transform.DORotate(new Vector3(rotateAngle, 0, 0), rotateDuration, RotateMode.LocalAxisAdd));

        // 等待一段时间
        rotationSequence.AppendInterval(waitTime);

        // 反向旋转回去
        rotationSequence.Append(transform.DORotate(new Vector3(-rotateAngle, 0, 0), rotateDuration, RotateMode.LocalAxisAdd));

        // 等待一段时间
        rotationSequence.AppendInterval(waitTime);

        // 循环执行
        rotationSequence.SetLoops(-1, LoopType.Restart);  // -1 表示无限循环，LoopType.Restart 表示每次循环从头开始
    }
}
