using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

/// <summary>
///     Singleton that manages the game state.
/// </summary>
public class GameManager : Singleton<GameManager>
{
    /// <summary>
    ///     Toggle whether the game is paused.
    /// </summary>
    public void TogglePause() {
        if (Time.timeScale <= 0)
            ResumeGame();
        else
            PauseGame();
    }

    /// <summary>
    ///     Pause the game.
    /// </summary>
    public void PauseGame() {
        Time.timeScale = 0;
    }

    /// <summary>
    ///     Resume the game.
    /// </summary>
    public void ResumeGame() {
        Time.timeScale = 1;
    }
    
    public void HitStop(float duration = 0.1f)
    {
        StartCoroutine(HitStopRoutine(duration));
    }

    private IEnumerator HitStopRoutine(float duration)
    {
        Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(duration);
        Time.timeScale = 1;
    }

    /// <summary>
    ///     Exit the game.
    /// </summary>
    public void ExitGame() {
        Application.Quit();
    }
}