using KBCore.Refs;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : ValidatedMonoBehaviour
{
    [Header("References")]
    [SerializeField, Anywhere] InputReader input;
    // [SerializeField, Anywhere] GameObject gameOverUI;  // 游戏结束UI
    [SerializeField, Anywhere] GameObject pauseMenuUI; //游戏暂停菜单UI

    public static GameManager Instance; // 单例模式
    public Animator animator;
    public int score { get; private set; } // 分数
    

    // 游戏是否结束
    private bool isGameOver = false; // 私有字段，外界无法直接访问
    private bool gameIsPaused = false;

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

    public void AddScore(int score)
    {
        if (isGameOver) return;
        this.score += score;
    }

    void OnEnable()
    {
        input.Menu += OnMenu;
    }

    void OnDisable()
    {
        input.Menu -= OnMenu;
    }

    private void OnMenu()
    {
        if(pauseMenuUI == null)
            return;

        if (gameIsPaused)
        {
            Resume();
        }
        else
        {
            Pause();
        }
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        gameIsPaused = false;

    }

    void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        gameIsPaused = true;

    }

    public void QuitGame()
    {
        Time.timeScale = 1f;
        StartCoroutine(LoadScene("LevelSelection"));
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
