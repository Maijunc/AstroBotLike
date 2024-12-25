using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance; // 单例模式
    public GameObject gameOverUI;       // 游戏结束UI

    // 游戏是否结束
    private bool isGameOver = false; // 私有字段，外界无法直接访问

    // 只读属性，外界可以读取但无法修改
    public bool IsGameOver
    {
        get { return isGameOver; }
    }    

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
    }


}
