using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class BridgeCreateScript : MonoBehaviour
{
    public GameObject Container;//���ɺ���������

    public GameObject landPrefab;  // �������ɵ�Ԥ����
    public int landRowCount = 1;      // ���е���
    public int landColCount = 1;      // ���е���
    public float generateDistanceX = 0f;//�봥����X��ľ���
    public float generateDistanceY = 0f;//�봥����Y��ľ���
    public float generateDistanceZ = 0f;//�봥����Z��ľ���
    public float distance = 1f;    // ���ɵ�½�ؼ��
    public float delayTime = 0.5f;// ����ÿ��½�ص��ӳ�ʱ��
    public float riseDuration = 3f; // ½�������ʱ��
    public float riseHeight = 4f;  // ½������ĸ߶�

    private bool ifTrigge = false;//�Ƿ񴥷���

    void OnCollisionEnter(Collision collision)
    {
        // �ж��Ƿ���Ŀ�����崥������
        if (collision.gameObject.tag == "Player" && !ifTrigge)
        {
            GenerateLand();
            ifTrigge = true;
        }
    }

    // ����½��
    void GenerateLand()
    {
        // �ӵ�ǰ�����λ�ü��ϸ����ƫ�ƴ���ʼ����½��
        Vector3 startPosition = transform.position + new Vector3(generateDistanceX, generateDistanceY, generateDistanceZ);
        List<GameObject> generatedLands = new List<GameObject>();  // ���ڱ������ɵ�½��

        for (int row = 0; row < landRowCount; row++)
        {
            for (int col = 0; col < landColCount; col++)
            {
                // ����½�ص�����λ��
                Vector3 position = startPosition + new Vector3(col * distance, 0, row * distance);

                // ʵ����Ԥ����
                GameObject land = Instantiate(landPrefab, position, Quaternion.identity);
                generatedLands.Add(land);
                land.transform.parent = Container.transform;

                //ÿ��½�ص��ӳ�ʱ����Ը�������˳��������
                float nowDelayTime = (row * landColCount + col) * delayTime;

                DG.Tweening.DOTween.SetTweensCapacity(tweenersCapacity: 200, sequencesCapacity: 200);
                // ʹ��DOTween��ÿ��½�ش����������𣬲�����ÿ��½�ض������ӳ�
                DOVirtual.DelayedCall(nowDelayTime, () =>
                {
                    // ���ӳٺ�ִ��½�ض���
                    land.SetActive(true);  // �ڶ�����ʼǰ��������
                    land.transform.DOMoveY(land.transform.position.y + riseHeight, riseDuration)
                        .SetEase(Ease.OutBounce);  // ʹ�û���Ч��
                });
            }
        }
    }
}
