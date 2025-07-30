using System.Collections;
using UnityEngine.SceneManagement;

public class SceneChanger : Singleton<SceneChanger>
{
    public void ChangeScene(string sceneName)
    {
        StartCoroutine(ChangeSceneRoutine(sceneName));
    }

    private IEnumerator ChangeSceneRoutine(string sceneName)
    {
        var loadOp = SceneManager.LoadSceneAsync(sceneName);
        var fader = UIManager.Instance.GetUI<Fader>();
        yield return fader.FadeIn();
        yield return loadOp;
        yield return fader.FadeOut();
    } 
}