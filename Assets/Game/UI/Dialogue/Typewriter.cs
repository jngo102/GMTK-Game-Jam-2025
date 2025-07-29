using TMPro;
using UnityEngine;

/// <summary>
///     A typewriter effect for a TextMeshProUGUI.
/// </summary>
[RequireComponent(typeof(TextMeshProUGUI))]
public class Typewriter : MonoBehaviour {
    [SerializeField] private float printInterval = 0.1f;

    /// <summary>
    ///     A timer that tracks whether the next character should be printed.
    /// </summary>
    private float printTimer;

    /// <summary>
    ///     The current index of the text that has been printed.
    /// </summary>
    private int textIndex;

    private TextMeshProUGUI textObject;
    public string Text { get; set; }

    /// <summary>
    ///     Whether the typewriter is currently printing out text.
    /// </summary>
    public bool IsPrinting { get; private set; }

    private void Awake() {
        textObject = GetComponent<TextMeshProUGUI>();
    }

    private void Update() {
        if (Text == null || !IsPrinting) return;

        printTimer += Time.deltaTime;
        if (printTimer >= printInterval) {
            printTimer = 0;
            textIndex++;
            textObject.text = Text[..textIndex];
        }

        if (textIndex >= Text.Length) StopTyping();
    }

    /// <summary>
    ///     Type out the given text.
    /// </summary>
    /// <param name="text">The text to type out.</param>
    public void Type(string text) {
        IsPrinting = true;
        printTimer = 0;
        textIndex = 0;
        textObject.text = "";
        Text = text;
    }

    /// <summary>
    ///     Skip the typewriter effect and print out the entire text.
    /// </summary>
    public void Skip() {
        textIndex = Text.Length;
        textObject.text = Text;
        StopTyping();
    }

    /// <summary>
    ///     Stop printing out text.
    /// </summary>
    private void StopTyping() {
        IsPrinting = false;
    }
}