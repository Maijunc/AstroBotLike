using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NewEmptyCSharpScript : MonoBehaviour
{
    public Button startButton;
    public Button quitButton;

    void Start()
    {
        // 设置按钮的点击事件
        startButton.onClick.AddListener(StartGame);
        quitButton.onClick.AddListener(QuitGame);
    }

    // 开始游戏
    void StartGame()
    {
        // 假设游戏的第一个场景是"GameScene"，你可以根据实际情况修改
        SceneManager.LoadScene("LevelSelection");
    }

    // 退出游戏
    void QuitGame()
    {
        // 如果是PC或Mac平台，退出应用
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }
}
