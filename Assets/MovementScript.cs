using UnityEngine;

public class MovementScript : MonoBehaviour
{
    public float speed;
    // 旋转速度
    public float rotationSpeed;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        // 获取水平方向的输入（通常是键盘的 A/D 键或箭头键）
        float horizontalMove = Input.GetAxis("Horizontal");
        float verticalMove = Input.GetAxis("Vertical");

        Vector3 moveDirection = new Vector3(horizontalMove, 0, verticalMove);
        // 将 moveDirection 向量的长度调整为 1，而不改变其方向
        // Normalize的作用是当玩家按下两个按键，比如"WD"，moveDirection > 1，防止速度过快
        moveDirection.Normalize();
        float magnitude = moveDirection.magnitude;
        magnitude = Mathf.Clamp01(magnitude);

        // Space.World 使用世界坐标系进行移动 无论物体如何旋转，它的移动方向始终与世界坐标系对齐
        transform.Translate(moveDirection * magnitude * speed * Time.deltaTime, Space.World);
    
        // 避免在没有移动时物体无缘无故地进行旋转。
        if(moveDirection != Vector3.zero)
        {
            // moveDirection 表示物体希望朝向的方向，而 Vector3.up 确保旋转是围绕世界坐标系的 y 轴进行的
            Quaternion toRotate = Quaternion.LookRotation(moveDirection, Vector3.up);
            // 平滑旋转
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotate, rotationSpeed * Time.deltaTime);
        }
    }
}
