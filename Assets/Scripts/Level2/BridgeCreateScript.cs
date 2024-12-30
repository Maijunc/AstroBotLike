using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class BridgeCreateScript : MonoBehaviour
{
    public GameObject Container;//生成后放入的容器

    public GameObject landPrefab;  // 用于生成的预制体
    public int landRowCount = 1;      // 阵列的行
    public int landColCount = 1;      // 阵列的列
    public float generateDistanceX = 0f;//离触发点X轴的距离
    public float generateDistanceY = 0f;//离触发点Y轴的距离
    public float generateDistanceZ = 0f;//离触发点Z轴的距离
    public float distance = 1f;    // 生成的陆地间距
    public float delayTime = 0.5f;// 计算每个陆地的延迟时间
    public float riseDuration = 3f; // 陆地升起的时间
    public float riseHeight = 4f;  // 陆地升起的高度

    private bool ifTrigge = false;//是否触发过

    void OnCollisionEnter(Collision collision)
    {
        // 判断是否是目标物体触发生成
        if (collision.gameObject.tag == "Player" && !ifTrigge)
        {
            GenerateLand();
            ifTrigge = true;
        }
    }

    // 生成陆地
    void GenerateLand()
    {
        // 从当前物体的位置加上各轴的偏移处开始生成陆地
        Vector3 startPosition = transform.position + new Vector3(generateDistanceX, generateDistanceY, generateDistanceZ);
        List<GameObject> generatedLands = new List<GameObject>();  // 用于保存生成的陆地

        for (int row = 0; row < landRowCount; row++)
        {
            for (int col = 0; col < landColCount; col++)
            {
                // 计算陆地的生成位置
                Vector3 position = startPosition + new Vector3(col * distance, 0, row * distance);

                // 实例化预制体
                GameObject land = Instantiate(landPrefab, position, Quaternion.identity);
                generatedLands.Add(land);
                land.transform.parent = Container.transform;

                //每个陆地的延迟时间可以根据行列顺序来计算
                float nowDelayTime = (row * landColCount + col) * delayTime;

                DG.Tweening.DOTween.SetTweensCapacity(tweenersCapacity: 200, sequencesCapacity: 200);
                // 使用DOTween让每个陆地从下往上升起，并控制每个陆地动画的延迟
                DOVirtual.DelayedCall(nowDelayTime, () =>
                {
                    // 在延迟后执行陆地动画
                    land.SetActive(true);  // 在动画开始前激活物体
                    land.transform.DOMoveY(land.transform.position.y + riseHeight, riseDuration)
                        .SetEase(Ease.OutBounce);  // 使用缓动效果
                });
            }
        }
    }
}
