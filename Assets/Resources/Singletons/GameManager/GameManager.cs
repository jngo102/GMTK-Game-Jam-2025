using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

/// <summary>
///     Singleton that manages the game state.
/// </summary>
public class GameManager : Singleton<GameManager>
{
    public UnityAction LevelStarted;
    
    /// <summary>
    ///     Change scenes with a fade transition.
    /// </summary>
    /// <param name="sceneName">The name of the scene to change to.</param>
    /// <param name="sceneTransitionType">The type of scene transition when changing scenes.</param>
    /// <param name="entryName">The name of the scene transition trigger to enter from after the scene changes.</param>
    public void ChangeScene(string sceneName) {//, SceneTransitionType sceneTransitionType = SceneTransitionType.Level, string entryName = null) {
        StartCoroutine(ChangeSceneRoutine(sceneName));//, sceneTransitionType, entryName));
    }

    /// <summary>
    ///     The routine that carries out the scene change sequence.
    /// </summary>
    /// <param name="sceneName">The name of the scene to change to.</param>
    /// <param name="sceneTransitionType">The type of scene transition when changing scenes.</param>
    /// <param name="entryName">The name of the scene transition trigger to load from.</param>
    /// <returns></returns>
    public IEnumerator ChangeSceneRoutine(string sceneName) {//, SceneTransitionType sceneTransitionType, string entryName) {
        var fader = UIManager.Instance.GetUI<Fader>();
        yield return fader.FadeIn();
        SaveManager.SaveGame();
        yield return SceneManager.LoadSceneAsync(sceneName);
        SaveManager.LoadGame();
        // switch (sceneTransitionType) {
        //     case SceneTransitionType.Level when SceneData.IsGameplayScene(sceneName) && entryName != null:
        //         StartLevel(entryName);
        //         break;
        //     case SceneTransitionType.MainMenu when SceneData.IsGameplayScene(sceneName):
        //         StartLevel();
        //         break;
        // }
        
        LevelStarted?.Invoke();
        
        yield return fader.FadeOut();
    }
    
    /// <summary>
    ///     Toggle whether the game is paused.
    /// </summary>
    public static void TogglePause() {
        if (Time.timeScale <= 0)
            ResumeGame();
        else
            PauseGame();
    }

    /// <summary>
    ///     Pause the game.
    /// </summary>
    public static void PauseGame() {
        Time.timeScale = 0;
    }

    /// <summary>
    ///     Resume the game.
    /// </summary>
    public static void ResumeGame() {
        Time.timeScale = 1;
    }

    /// <summary>
    ///     Exit the game.
    /// </summary>
    public static void ExitGame() {
        Application.Quit();
    }
}