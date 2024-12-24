using UnityEngine;

public class MovementScript : MonoBehaviour
{
    public float speed;
    // 旋转速度
    public float rotationSpeed;
    // 跳跃速度
    public float jumpSpeed;
    public float ySpeed;

    // 喷气背包
    public float jetpackForce = 3f;        // 喷气背包的上升力
    public float jetpackDuration = 0.5f;     // 喷气背包的持续时间

    [SerializeField]
    private float jetpackTimeRemaining; // 剩余的喷气背包时间
    private bool isJetpacking = false; // 是否正在使用喷气背包

    private CharacterController conn;
    public bool isGrounded;

    private Vector3 velocity; //速度
    public Joystick joy;

    void Start()
    {
        conn = GetComponent<CharacterController>();
        jetpackTimeRemaining = jetpackDuration;
    }

    // Update is called once per frame
    void Update()
    {
        MoveCharacter();

        // 喷气背包控制：跳跃时按下跳跃键触发喷气背包
        if (Input.GetButtonDown("Jump") && !isGrounded && !isJetpacking && jetpackTimeRemaining > 0)
        {
            StartJetpack();
        }

        // 如果松开跳跃键，停止喷气背包
        if (Input.GetButtonUp("Jump") && isJetpacking)
        {
            StopJetpack();  // 停止喷气背包
        }

        // 喷气背包持续时间
        if (isJetpacking)
        {
            jetpackTimeRemaining -= Time.deltaTime;
            if (jetpackTimeRemaining <= 0f)
            {
                StopJetpack();
            }
        } else if(isGrounded && jetpackTimeRemaining < jetpackDuration) // 如果落地且没有开启背包且背包时间没满，开始充能
        {
            jetpackTimeRemaining += Time.deltaTime;
        }
    }

    // 移动角色
    void MoveCharacter()
    {
        // 获取水平方向的输入（通常是键盘的 A/D 键或箭头键）
        float horizontalMove = Input.GetAxis("Horizontal");
        float verticalMove = Input.GetAxis("Vertical");

        float joyHorizontalMove = joy.Horizontal * speed;
        float joyVerticalMove = joy.Vertical * speed;

        Vector3 moveDirection = new Vector3(horizontalMove, 0, verticalMove);
        // 将 moveDirection 向量的长度调整为 1，而不改变其方向
        // Normalize的作用是当玩家按下两个按键，比如"WD"，moveDirection > 1，防止速度过快
        moveDirection.Normalize();
        float magnitude = moveDirection.magnitude;
        magnitude = Mathf.Clamp01(magnitude);

        Vector3 joyMovement = new Vector3(joyHorizontalMove, 0, joyVerticalMove);
        joyMovement.Normalize();
        float joyMagnitude = joyMovement.magnitude;
        joyMagnitude = Mathf.Clamp01(joyMagnitude);

        // 更新ySpeed（受重力影响）
        if (!isJetpacking)
        {
            ySpeed += Physics.gravity.y * Time.deltaTime;
        } else {
            ySpeed = jetpackForce;  // 喷气背包增加向上的速度
        }

        velocity = moveDirection != Vector3.zero ? moveDirection * magnitude * speed : joyMovement * joyMagnitude * speed;

        if (conn.isGrounded)
        {
            ySpeed = -0.5f;  // 在地面时施加小的负值，避免角色“浮动”
            isGrounded = true;

            // 按下跳跃键，进行跳跃
            if (Input.GetButtonDown("Jump") && !isJetpacking)
            {
                ySpeed = jumpSpeed;
                isGrounded = false;
            }
        } else {
            isGrounded = false;
        }
        velocity.y = ySpeed;

        conn.Move(velocity * Time.deltaTime);

        // 避免在没有移动时物体无缘无故地进行旋转。
        if(moveDirection != Vector3.zero)
        {
            // moveDirection 表示物体希望朝向的方向，而 Vector3.up 确保旋转是围绕世界坐标系的 y 轴进行的
            Quaternion toRotate = Quaternion.LookRotation(moveDirection, Vector3.up);
            // 平滑旋转
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotate, rotationSpeed * Time.deltaTime);
        }

        // 避免在没有移动时物体无缘无故地进行旋转。
        if(joyMovement != Vector3.zero)
        {
            // moveDirection 表示物体希望朝向的方向，而 Vector3.up 确保旋转是围绕世界坐标系的 y 轴进行的
            Quaternion toRotate = Quaternion.LookRotation(joyMovement   , Vector3.up);
            // 平滑旋转
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotate, rotationSpeed * Time.deltaTime);
        }
    }

    // 启动喷气背包
    void StartJetpack()
    {
        isJetpacking = true;
        // jetpackTimeRemaining = jetpackDuration;  // 设置喷气背包的持续时间
    }

    // 停止喷气背包
    void StopJetpack()
    {
        isJetpacking = false;
        ySpeed = 0;  // 停止喷气背包时，停止向上的速度
    }
    
}
