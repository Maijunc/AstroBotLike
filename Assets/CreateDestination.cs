using DG.Tweening;
using UnityEngine;
using System.Collections;

public class CreateDestination : MonoBehaviour
{
    public GameObject destinationObject;//Ŀ�ĵ�����
    public float[] destinationPositions;//Ŀ�ĵ����ɵ�����
    public float destinationMoveSpeed = 0f;//Ŀ�ĵ��ƶ����ٶ�
    public float destinationMoveHeight = 0f;//Ŀ�ĵ��ƶ��ĸ߶�
    public float destinationWaitTime;//Ŀ�ĵصȴ�����ʱ�������


    private bool ifTriggered = false;

    void OnTriggerEnter(Collider other)
    {
        // ȷ�����������ǽ�ɫ
        if (other.CompareTag("Player") && !ifTriggered)
        {
            Debug.Log("��ɫ��Խ�����崥����!");
            // ������ִ����Ľű��߼��������������塢���Ŷ�����
            StartCreate();

            ifTriggered = true;
        }
    }

    private void StartCreate() {
        StartCoroutine(CreateDestnationObject());
    }


    IEnumerator CreateDestnationObject()
    {
        // �ȴ�ָ��ʱ��
        yield return new WaitForSeconds(destinationWaitTime);

        // ����Ŀ�ĵص�����
        Vector3 startPosition = new Vector3(destinationPositions[0], destinationPositions[1], destinationPositions[2]);
        Vector3 targetPosition = new Vector3(destinationPositions[0], destinationPositions[1] + destinationMoveHeight, destinationPositions[2]);

        // ��������
        GameObject destination = Instantiate(destinationObject, startPosition, Quaternion.identity);

        // ʹ�� DoTween ʹ������ Y ��ƽ���ƶ���Ŀ��λ��
        destination.transform.DOMoveY(targetPosition.y, destinationMoveHeight / destinationMoveSpeed)
            .SetEase(Ease.Linear); // ʹ���������ٵ��ƶ���ʽ
    }
}
