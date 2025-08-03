using System.Collections;
using TMPro;
using UnityEngine;

public class WinScreen : MonoBehaviour
{
    public Scoreboard scoreboard;
    public TextMeshProUGUI newHighScoreText;
    
    private void OnEnable()
    {
        StartCoroutine(Win());
    }
    
    private IEnumerator Win()
    {
        if (scoreboard.CurrentScore >= SaveManager.SaveData.highScore)
        {
            newHighScoreText.gameObject.SetActive(true);
            SaveManager.SaveData.highScore = scoreboard.CurrentScore;
        }
        yield return new WaitForSeconds(5);
        SceneChanger.Instance.ChangeScene("Main Menu");
    }
}
