using UnityEngine;
using System.Collections;

public class Radar : MonoBehaviour
{
    // 目标点Transform
    public Transform target_trans;
    // 运动速度
    public float speed = 10;
    // 最小接近距离, 以停止运动
    public float min_distance = 0.5f;
    private float distanceToTarget;
    private bool move_flag = true;
    private Transform m_trans;

    void Start()
    {
        m_trans = this.transform;
        distanceToTarget = Vector3.Distance(m_trans.position, target_trans.position);
        StartCoroutine(Parabola());
    }

    IEnumerator Parabola()
    {
        Vector3 targetPos = target_trans.position;
        while (move_flag)
        {
            
            // 朝向目标, 以计算运动
            m_trans.LookAt(targetPos);

            float distanceFactor = Vector3.Distance(m_trans.position, targetPos) / distanceToTarget;
            float angle = Mathf.Min(1, distanceFactor) * 50f;  // 增加最大角度至 90°

            // 旋转对应的角度（线性插值一定角度，然后每帧绕X轴旋转）
            m_trans.rotation = m_trans.rotation * Quaternion.Euler(Mathf.Clamp(-angle, -90, 90), 0, 0);  // 改变角度限制为 -90 到 90


            // 当前距离目标点
            float currentDist = Vector3.Distance(m_trans.position, target_trans.position);
            
            // 平移 (朝向Z轴移动)
            m_trans.Translate(Vector3.forward * Mathf.Min(speed * Time.deltaTime, currentDist));

            // 很接近目标了, 准备结束循环
            if (currentDist < min_distance)
            {
                move_flag = false;
            }
            // 暂停执行, 等待下一帧再执行while
            yield return null;
        }
        // 销毁脚本
        GameObject.Destroy(this);
        // [停止]当前协程任务,参数是协程方法名
        StopCoroutine(Parabola());
        
    }
}