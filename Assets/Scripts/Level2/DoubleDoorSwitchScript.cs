using DG.Tweening;
using System;
using UnityEngine;

public class DoubleDoorOpen:MonoBehaviour
{
    public GameObject LeftDoor;
    public GameObject RightDoor;
    public bool Type=false;//false to OnCollisionEnter,true to OnTriggerEnter

    public float moveLeftDistance=0f;
    public float moveRightDistance = 0f;
    public float moveDuration = 1f; // 移动持续时间

    private bool ifTrigger = false;

    void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.tag == "Player"&& !ifTrigger && !Type) {
            ifTrigger = true;
            // 使用DoTween平滑移动左门和右门
            LeftDoor.transform.DOLocalMoveX(LeftDoor.transform.localPosition.x - moveLeftDistance, moveDuration);
            RightDoor.transform.DOLocalMoveX(RightDoor.transform.localPosition.x + moveRightDistance, moveDuration);
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.tag == "Player" && !ifTrigger && Type)
        {
            ifTrigger = true;
            // 使用DoTween平滑移动左门和右门
            LeftDoor.transform.DOLocalMoveX(LeftDoor.transform.localPosition.x - moveLeftDistance, moveDuration);
            RightDoor.transform.DOLocalMoveX(RightDoor.transform.localPosition.x + moveRightDistance, moveDuration);
        }
    }
}
