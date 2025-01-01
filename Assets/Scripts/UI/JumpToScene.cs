using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class JumpToScene : MonoBehaviour
{
    public Button LevelButton;
    public string LevelName;
    public Animator animator;

    void Start()
    {
        LevelButton.onClick.AddListener(StartGame);
    }

    void StartGame()
    {
        // 假设游戏的第一个场景是"GameScene"，你可以根据实际情况修改
        StartCoroutine(LoadScene(LevelName));
    }

    IEnumerator LoadScene(string sceneName)
    {
        animator.SetBool("FadeIn", true);
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
