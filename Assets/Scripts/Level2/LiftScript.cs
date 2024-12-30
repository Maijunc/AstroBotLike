using DG.Tweening;
using UnityEngine;

public class LiftScript : MonoBehaviour
{
    public bool ifLoop = true;//是否循环

    public float moveDistance = 1f;//移动的距离
    public float moveDuration = 1f;//移动的时间
    public float waitTime = 1f;//等待的时间

    public Vector3 moveDirection = Vector3.up;  // 移动方向，默认为向上
    private Vector3 startPosition;  // 物体的初始位置

    private void Start()
    {
        startPosition = transform.position;  // 记录物体的初始位置
        MoveLift();  // 开始移动
    }

    private void MoveLift()
    {
        // 计算目标位置
        Vector3 targetPosition = startPosition + moveDirection.normalized * moveDistance;

        // 使用 DOTween 进行平滑移动
        if (ifLoop)
        {
            // 往返移动：先移动到目标位置，再返回原位置
            transform.DOMove(targetPosition, moveDuration)
                .SetEase(Ease.Linear)  // 使用线性过渡
                .OnComplete(() =>  // 移动完成后回调
                {
                    // 等待一段时间，然后返回
                    DOVirtual.DelayedCall(waitTime, () =>
                    {
                        transform.DOMove(startPosition, moveDuration)
                            .SetEase(Ease.Linear)  // 使用线性过渡
                            .OnComplete(() =>
                            {
                                // 等待一段时间，然后再次移动
                                DOVirtual.DelayedCall(waitTime, MoveLift);
                            });
                    });
                });
        }
        else
        {
            // 单次移动：移动到目标位置后停止
            transform.DOMove(targetPosition, moveDuration)
                .SetEase(Ease.Linear)
                .OnComplete(() =>
                {
                    // 可以选择停止在目标位置或继续进行某些操作
                    // 在此不做其他操作，移动完成后结束
                });
        }
    }
}
