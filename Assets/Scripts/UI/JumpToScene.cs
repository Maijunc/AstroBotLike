using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class JumpToLevel : MonoBehaviour
{
    public Button LevelButton;
    public string LevelName;

    void Start()
    {
        LevelButton.onClick.AddListener(StartGame);
    }

    void StartGame()
    {
        // 假设游戏的第一个场景是"GameScene"，你可以根据实际情况修改
        SceneManager.LoadScene(LevelName);
    }
}
