using UnityEngine;
using System.Collections;
using System.Linq;
using Unity.VisualScripting;


public class CreateEnemy : MonoBehaviour
{
    public GameObject[] monsterPrefabs;  // 怪物预制体数组
    public Transform[] spawnPoints;    // 每个怪物的位置
    public GameObject enemyContainer;

    public GameObject movingObject;   // 需要控制移动的物体
    public Vector3 moveTarget;        // 物体目标位置
    public float moveSpeed = 5f;      // 移动速度

    private bool hasCrossedAirWall = false;  // 角色是否已穿过空气墙
    private bool ifFinishCreate = false;


    private void Update()
    {
        if (hasCrossedAirWall&& enemyContainer.transform.childCount == 0&& ifFinishCreate) {
            // 所有怪物被消灭后，开始移动物体
            Debug.Log("终点出现");
            StartCoroutine(MoveObjectToTarget());
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // 检测角色是否穿过空气墙
        if (other.CompareTag("Player")&& !hasCrossedAirWall)
        {
            hasCrossedAirWall = true;
            StartCoroutine(SpawnMonsters());
        }
    }

    private IEnumerator SpawnMonsters()
    {
        for(int i=0;i< monsterPrefabs.Length;i++)
        {
            GameObject enemy = Instantiate(monsterPrefabs[i], spawnPoints[i].position, Quaternion.identity);
            enemy.transform.parent = enemyContainer.transform;
        }
        ifFinishCreate = true;
        yield return null;
    }


    // 控制物体移动到目标位置
    private IEnumerator MoveObjectToTarget()
    {
        yield return new WaitForSeconds(3);

        Vector3 startPosition = movingObject.transform.position;
        float journeyLength = Vector3.Distance(startPosition, moveTarget);
        float startTime = Time.time;

        while (Vector3.Distance(movingObject.transform.position, moveTarget) > 0.1f)
        {
            float distanceCovered = (Time.time - startTime) * moveSpeed;
            float fractionOfJourney = distanceCovered / journeyLength;
            movingObject.transform.position = Vector3.Lerp(startPosition, moveTarget, fractionOfJourney);

            yield return null;
        }

        // 确保物体完全到达目标
        movingObject.transform.position = moveTarget;
    }
}
