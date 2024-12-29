using DG.Tweening;
using UnityEngine;

public class DrumScript : MonoBehaviour
{
    public float rotationSpeed = 90f;  // 旋转速度，单位：度/秒
    [SerializeField] private Vector3 rotationAxis = Vector3.up;
    void Update()
    {
        // 每一帧旋转一定的角度
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
    }
}
