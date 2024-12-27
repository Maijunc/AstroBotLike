using UnityEngine;
public class ScoreUI : MonoBehaviour 
{
    [SerializeField] TMPro.TextMeshProUGUI scoreText;

    void Start()
    {
        UpdateScore();
    }
    
    public void UpdateScore()
    {
        // Make sure all logic has run before updating the score
        StartCoroutine(UpdateScoreNextFrame());
    }

    System.Collections.IEnumerator UpdateScoreNextFrame()
    {
        yield return null;
        scoreText.text = GameManager.Instance.score.ToString();
    }
}
