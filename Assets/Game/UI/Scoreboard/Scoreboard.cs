using TMPro;
using UnityEngine;

/// <summary>
///     A scoreboard that displays the player's current score.
/// </summary>
public class Scoreboard : BaseUI {
    private int currentScore;

    [SerializeField] private TextMeshPro scoreValue;
    
    /// <summary>
    ///     The player's current score.
    /// </summary>
    public int CurrentScore {
        get => currentScore;
        private set {
            currentScore = value;
            scoreValue.text = currentScore.ToString();
        }
    }

    /// <summary>
    ///     Add to the current score.
    /// </summary>
    /// <param name="score">The score amount to add.</param>
    public void AddScore(int score) {
        CurrentScore += score;
    }

    /// <summary>
    ///     Subtract from the current score.
    /// </summary>
    /// <param name="score">The score amount to subtract.</param>
    public void SubtractScore(int score) {
        CurrentScore -= score;
    }
    
    /// <summary>
    ///     Reset the current score to 0.
    /// </summary>
    public void ResetScore() {
        CurrentScore = 0;
    }
}