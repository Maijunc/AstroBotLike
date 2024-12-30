using DG.Tweening;
using UnityEngine;

public class PushBoxGame : MonoBehaviour
{
    public GameObject LeftDoor;//控制大门开启
    public GameObject RightDoor;

    public GameObject[] targetObject;//目标物体

    private int need;//需要触发的个数
    private int nowPushNum=0;//现在完成的个数
    private bool ifFinished = false;//是否已经完成游戏任务

    public float moveLeftDistance = 0f;
    public float moveRightDistance = 0f;
    public float moveDuration = 1f; // 移动持续时间

    void Start()
    {
        // 根据目标物体的数量确定需要触发的个数
        need = targetObject.Length;
    }

    void OnCollisionEnter(Collision collision)
    {
        // 判断是否是目标物体触发并且任务尚未完成
        foreach (GameObject target in targetObject)
        {
            if (collision.gameObject == target && !ifFinished)
            {
                // 确保每个目标物体触发一次碰撞后，不再重复触发
                if (!target.GetComponent<Collider>().isTrigger)
                {
                    target.GetComponent<Collider>().enabled = false; // 禁用碰撞器，防止再触发

                    // 增加完成的物体数量
                    nowPushNum++;
                    Debug.Log("nowPushNum: " + nowPushNum);

                    // 判断是否完成所有任务
                    if (nowPushNum == need)
                    {
                        Debug.Log("Push box finished");
                        ifFinished = true;
                        // 使用DoTween平滑移动左门和右门
                        LeftDoor.transform.DOLocalMoveX(LeftDoor.transform.localPosition.x - moveLeftDistance, moveDuration);
                        RightDoor.transform.DOLocalMoveX(RightDoor.transform.localPosition.x + moveRightDistance, moveDuration);
                    }

                    break; // 只处理一次碰撞
                }
            }
        }
    }
}
