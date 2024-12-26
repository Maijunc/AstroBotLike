using System.Collections;
using UnityEngine;
using DG.Tweening;

public class LandFallDown : MonoBehaviour
{
    public float initialSinkDistance = 0.25f;  // 初始下沉的距离
    public float sinkDuration = 1f;           // 缓慢下沉所需的时间
    public float constantSpeed = 2.5f;        // 恒定下沉的速度（单位：单位/秒）
    public float destroyHeight = -15f;         // 物体销毁的Y轴位置

    private bool isSinking = false;

    private void OnCollisionEnter(Collision collision)
    {
        if (!isSinking)
        {
            // 检测到碰撞后触发下沉
            StartSink();
        }
    }

    private void StartSink()
    {
        isSinking = true;

        // 初始下沉：使用 DOTween 缓慢下沉一定的距离
        transform.DOMoveY(transform.position.y - initialSinkDistance, sinkDuration)
            .OnComplete(() => StartConstantSink()); // 完成初始下沉后，开始恒定速度下沉
    }

    private void StartConstantSink()
    {
        // 使用协程以恒定速度下沉
        StartCoroutine(ConstantSinkCoroutine());
    }

    private IEnumerator ConstantSinkCoroutine()
    {
        while (transform.position.y > destroyHeight)
        {
            transform.position += Vector3.down * constantSpeed * Time.deltaTime;
            yield return null;
        }

        // 一旦下沉到目标高度，销毁物体
        Destroy(gameObject);
    }
}
