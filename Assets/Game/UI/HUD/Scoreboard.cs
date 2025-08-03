using System;
using TMPro;
using UnityEngine;

/// <summary>
///     A scoreboard that displays the player's current score.
/// </summary>
public class Scoreboard : MonoBehaviour {
    private int currentScore;

    [SerializeField] private TextMeshProUGUI scoreValue;
    [SerializeField] private TextMeshProUGUI multiplierValue;

    public float comboTime = 0.5f;

    private float comboTimer;
    private int multiplier = 1;
    
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

    public int Multiplier
    {
        get => multiplier;
        set
        {
            multiplier = value;
            if (value <= 1)
            {
                multiplierValue.text = "";
                return;
            }
            multiplierValue.text = $"x{value}";
        }
    }

    private void Awake()
    {
        ResetScore();
    }

    private void Update()
    {
        comboTimer += Time.deltaTime;
        if (comboTimer >= comboTime)
        {
            Multiplier = 1;
        }
    }

    /// <summary>
    ///     Add to the current score.
    /// </summary>
    /// <param name="score">The score amount to add.</param>
    public void AddScore(int score) {
        if (comboTimer < comboTime)
        {
            Multiplier++;
        }
        comboTimer = 0;
        CurrentScore += score * Multiplier;
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
        Multiplier = 1;
    }
}