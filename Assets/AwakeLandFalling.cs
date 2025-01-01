using DG.Tweening;
using UnityEngine;
using System.Collections;

public class AwakeLandFalling : MonoBehaviour
{
    public GameObject[] objectsToMove; // 要移动的物体
    public float destroyHeight = -20f;//下降的高度
    public float constantSpeed = 2.5f;        // 恒定下沉的速度（单位：单位/秒）
    public float[] waitTimes; // 每个物体的等待时间

    private bool ifTriggered = false;
    private bool ifFinished = false;

    void OnTriggerEnter(Collider other)
    {
        // 确保触发物体是角色
        if (other.CompareTag("Player")&& !ifTriggered)
        {
            Debug.Log("角色穿越了物体触发器!");
            ifTriggered = true;
        }
    }

    private void Update()
    {
        if (transform.childCount == 0&& !ifFinished)
        {
            // 在这里执行你的脚本逻辑，例如生成物体、播放动画等
            StartSink();
            ifFinished = true;
        }
    }

    private void StartSink() {
        // 确保等待时间数组长度与物体数量相同
        if (objectsToMove.Length != waitTimes.Length)
        {
            Debug.LogError("物体数量与等待时间数组长度不匹配!");
            return;
        }
        // 依次对每个物体进行平滑移动
        for (int i = 0; i < objectsToMove.Length; i++)
        {
            StartCoroutine(MoveObjectsWithDelay(i));
        }
        
    }

    // 协程，逐个物体平滑移动
    IEnumerator MoveObjectsWithDelay(int index)
    {
        // 等待指定时间
        yield return new WaitForSeconds(waitTimes[index]);
        // 让物体开始下降
        float startHeight = objectsToMove[index].transform.position.y;
        float distanceToMove = startHeight - destroyHeight;

        // 使用 DoTween 使物体下沉
        objectsToMove[index].transform.DOMoveY(destroyHeight, distanceToMove / constantSpeed)
            .SetEase(Ease.Linear) // 线性匀速下降
            .OnKill(() => Destroy(objectsToMove[index])); // 动画结束后销毁物体
    }
}
