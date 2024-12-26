using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class LandGeneratorOnWalking : MonoBehaviour
{
    public GameObject landPrefab;  // 用于生成的预制体
    private int landCount = 1;      // 生成的陆地数量
    public float generateDistanceX = 0f;//离触发点X轴的距离
    public float generateDistanceY = -5f;//离触发点Y轴的距离
    public float generateDistanceZ = 1.5f;//离触发点Z轴的距离
    public float distance = 1f;    // 生成的陆地间距
    public float riseDuration = 0.2f; // 陆地升起的时间
    public float riseHeight = 5f;  // 陆地升起的高度

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
        Vector3 startPosition = transform.position;  // 从当前物体的位置开始生成陆地
        List<GameObject> generatedLands = new List<GameObject>();  // 用于保存生成的陆地

        for (int i = 0; i < landCount; i++)
        {
            // 计算陆地的生成位置
            Vector3 position = startPosition + new Vector3(generateDistanceX, generateDistanceY, i * distance + generateDistanceZ);

            // 实例化预制体
            GameObject land = Instantiate(landPrefab, position, Quaternion.identity);
            generatedLands.Add(land);

            // 使用DOTween让每个陆地从下往上升起
            land.transform.DOMoveY(land.transform.position.y + riseHeight, riseDuration)
                .SetEase(Ease.OutBounce)  // 使用缓动效果
                .OnStart(() => land.SetActive(true));  // 在动画开始时激活物体
        }
    }
}
