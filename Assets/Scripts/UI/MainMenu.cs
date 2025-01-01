using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public Button startButton;
    public Button quitButton;
    public Animator animator;

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
        StartCoroutine(LoadScene("LevelSelection"));
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

    IEnumerator LoadScene(string sceneName) {
        animator.SetBool("FadeIn",true);
        animator.SetBool("FadeOut", false);

        yield return new WaitForSeconds(1);

        AsyncOperation async = SceneManager.LoadSceneAsync(sceneName);
        async.completed += OnLoadScene;
    }

    private void OnLoadScene(AsyncOperation operation)
    {
        animator.SetBool("FadeIn", false);
        animator.SetBool("FadeOut", true);
    }
}
