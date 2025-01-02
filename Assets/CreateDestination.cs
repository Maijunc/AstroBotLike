using DG.Tweening;
using UnityEngine;
using System.Collections;

public class CreateDestination : MonoBehaviour
{
    public GameObject destinationObject;//目的地物体
    public float[] destinationPositions;//目的地生成的坐标
    public float destinationMoveSpeed = 0f;//目的地移动的速度
    public float destinationMoveHeight = 0f;//目的地移动的高度
    public float destinationWaitTime;//目的地等待多少时间后生成


    private bool ifTriggered = false;
    private bool ifFinish = false;

    private void FixedUpdate()
    {
        if (transform.childCount == 0 && ifTriggered && !ifFinish)
        {
            StartCreate();
            ifFinish = true;
        }
    }

    void OnDestroy()
    {
        DOTween.KillAll();
    }

    void OnTriggerEnter(Collider other)
    {
        // 确保触发物体是角色
        if (other.CompareTag("Player") && !ifTriggered)
        {
            Debug.Log("角色穿越了物体触发器!");

            ifTriggered = true;
        }
    }

    private void StartCreate() {
        StartCoroutine(CreateDestnationObject());
    }


    IEnumerator CreateDestnationObject()
    {
        // 等待指定时间
        yield return new WaitForSeconds(destinationWaitTime);

        // 计算目的地的坐标
        Vector3 startPosition = new Vector3(destinationPositions[0], destinationPositions[1], destinationPositions[2]);
        Vector3 targetPosition = new Vector3(destinationPositions[0], destinationPositions[1] + destinationMoveHeight, destinationPositions[2]);

        // 生成物体
        GameObject destination = Instantiate(destinationObject, startPosition, Quaternion.identity);

        // 使用 DoTween 使物体沿 Y 轴平滑移动到目标位置
        destination.transform.DOMoveY(targetPosition.y, destinationMoveHeight / destinationMoveSpeed)
            .SetEase(Ease.Linear); // 使用线性匀速的移动方式
    }
}
