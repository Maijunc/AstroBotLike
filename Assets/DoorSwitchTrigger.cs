using UnityEngine;
using System.Collections;

public class DoorSwitchTrigger : MonoBehaviour
{
    public GameObject door; // 要移动的大门
    public Vector3 openPosition; // 开门后的目标位置
    public float delayTime = 2f; // 等待时间

    private bool playerOnTrigger = false; // 标记玩家是否在触发区
    private Coroutine doorCoroutine; // 记录正在执行的协程
    private Vector3 initialPosition; // 门的初始位置

    private bool doorIsAtTarget = false; // 标记门是否已经在目标位置

    void Start()
    {
        initialPosition = door.transform.position; // 记录初始位置
    }

    void Update()
    {
        // 如果门已经到达目标位置，更新标记
        if (door.transform.position == openPosition)
        {
            doorIsAtTarget = true;
        }
        else
        {
            doorIsAtTarget = false;
        }
    }

    void OnTriggerStay(Collider other)
    {
        // 检查玩家是否踩到物体
        if (other.CompareTag("Player"))
        {
            if (doorIsAtTarget) return; // 如果门已经在目标位置，什么都不做

            playerOnTrigger = true;

            // 启动协程，开启门
            if (doorCoroutine != null) // 防止多次启动协程
            {
                StopCoroutine(doorCoroutine);
                doorCoroutine = StartCoroutine(OpenDoorSmoothly());
            }
            else {
                doorCoroutine = StartCoroutine(OpenDoorSmoothly());
            }
                
        }
    }

    void OnTriggerExit(Collider other)
    {
        // 如果玩家离开触发器区域，重置触发状态
        if (other.CompareTag("Player"))
        {
            if (doorIsAtTarget) return; // 如果门已经在目标位置，什么都不做

            playerOnTrigger = false;

            // 如果门正在移动，停止并回到初始位置
            if (doorCoroutine != null)
            {
                StopCoroutine(doorCoroutine);
                doorCoroutine = StartCoroutine(CloseDoorSmoothly());
            }
        }
    }

    IEnumerator OpenDoorSmoothly()
    {
        Vector3 currentPosition = door.transform.position;
        float elapsedTime = 0f;

        // 门从当前位置平滑移动到目标位置
        while (elapsedTime < delayTime)
        {
            if (!playerOnTrigger) // 如果玩家离开触发区，停止移动
            {
                yield break; // 结束协程，停止门的移动
            }

            door.transform.position = Vector3.Lerp(currentPosition, openPosition, elapsedTime / delayTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // 确保门在目标位置
        door.transform.position = openPosition;
        doorIsAtTarget = true; // 标记门已在目标位置
    }

    IEnumerator CloseDoorSmoothly()
    {
        Vector3 currentPosition = door.transform.position;
        float elapsedTime = 0f;

        // 门从当前位置平滑移动回初始位置
        while (elapsedTime < delayTime)
        {
            if (playerOnTrigger) // 如果玩家进入触发区，停止移动
            {
                yield break; // 结束协程，停止门的移动
            }

            door.transform.position = Vector3.Lerp(currentPosition, initialPosition, elapsedTime / delayTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // 确保门回到初始位置
        door.transform.position = initialPosition;
        doorIsAtTarget = false; // 标记门已回到初始位置
    }
}
