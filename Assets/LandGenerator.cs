using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandGenerator : MonoBehaviour
{
    public GameObject Container;//生成后放入的容器

    public GameObject landPrefab;  // 用于生成的预制体
    public int landRowCount = 1;      // 阵列的行
    public int landColCount = 1;      // 阵列的列
    public float generateDistanceX = 4f;//离触发点X轴的距离
    public float generateDistanceY = 4f;//离触发点Y轴的距离
    public float generateDistanceZ = 4f;//离触发点Z轴的距离
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
        Vector3 startPosition = transform.position+ new Vector3(generateDistanceX, generateDistanceY,generateDistanceZ);  
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

                StartCoroutine(AnimateLand(land, nowDelayTime));

            }
        }

        // 协程来处理每个陆地的动画
        IEnumerator AnimateLand(GameObject land, float delayTime)
        {
            // 延迟一段时间
            yield return new WaitForSeconds(delayTime);

            // 激活陆地
            land.SetActive(true);

            // 记录陆地的初始位置
            Vector3 startPos = land.transform.position;

            // 计算动画开始和结束的时间
            float startTime = Time.time;
            float endTime = startTime + riseDuration;

            // 进行动画：通过改变位置实现从下往上的移动
            while (Time.time < endTime)
            {
                float t = (Time.time - startTime) / riseDuration;  // 计算当前时间的进度
                float yPos = Mathf.Lerp(startPos.y, startPos.y + riseHeight, t);  // 通过线性插值计算Y轴位置
                land.transform.position = new Vector3(startPos.x, yPos, startPos.z);
                yield return null;  // 等待下一帧
            }

            // 确保最终位置是正确的
            land.transform.position = new Vector3(startPos.x, startPos.y + riseHeight, startPos.z);
        }
    }
}
