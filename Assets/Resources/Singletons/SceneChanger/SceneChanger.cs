using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : Singleton<SceneChanger>
{
    public void ChangeScene(string sceneName)
    {
        StartCoroutine(ChangeSceneRoutine(sceneName));
    }

    private IEnumerator ChangeSceneRoutine(string sceneName)
    {
        var fader = UIManager.Instance.GetUI<Fader>();
        yield return fader.FadeIn();
        yield return SceneManager.LoadSceneAsync(sceneName);
        yield return fader.FadeOut();
    } 
}