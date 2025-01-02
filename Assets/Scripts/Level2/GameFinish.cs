using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameFinish : MonoBehaviour
{
    public Animator animator;
    public string nextScene;
    public bool ifBackToMenu=false;

    private bool ifTriggered = false;

    void OnTriggerEnter(Collider other)
    {
        // 确保触发物体是角色
        if (other.CompareTag("Player") && !ifTriggered)
        {
            Debug.Log("角色穿越了物体触发器!");
            if (ifBackToMenu){
                StartCoroutine(LoadScene("LevelSelection"));
            }
            else {
                if(nextScene==null)
                    StartCoroutine(LoadScene("LevelSelection"));
                else
                    StartCoroutine(LoadScene(nextScene));
            }
            ifTriggered = true;
        }
    }


    IEnumerator LoadScene(string sceneName)
    {
        animator.SetBool("FadeIn", true);
        animator.SetBool("FadeOut", false);

        yield return new WaitForSeconds(1);

        AsyncOperation async = SceneManager.LoadSceneAsync(sceneName);
    }
}
