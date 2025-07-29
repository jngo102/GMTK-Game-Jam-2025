using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
///     Manages dialogue from NPCs.
/// </summary>
public class DialogueManager : BaseUI {
    /// <summary>
    ///     The displayed dialogue typewriter.
    /// </summary>
    [SerializeField] private Typewriter typer;

    private Dialogue currentDialogue;
    private int currentPage;

    /// <summary>
    ///     The currently displayed, translated dialogue text.
    /// </summary>
    private string CurrentText => currentDialogue.pages[currentPage].GetLocalizedString();

    /// <inheritdoc />
    public override void Open() {
        base.Open();

        foreach (var playerInputManager in FindObjectsByType<PlayerInputHandler>(FindObjectsSortMode.None)) playerInputManager.Disable();

        if (UIManager.Instance) UIManager.Instance.Actions.Submit.performed += OnSubmit;
    }

    /// <inheritdoc />
    public override void Close() {
        currentDialogue = null;

        if (UIManager.Instance) UIManager.Instance.Actions.Submit.performed -= OnSubmit;

        foreach (var playerInputManager in FindObjectsByType<PlayerInputHandler>(FindObjectsSortMode.None)) playerInputManager.Enable();

        base.Close();
    }

    /// <summary>
    ///     Start a dialogue.
    /// </summary>
    /// <param name="dialogue">The dialogue data object.</param>
    public void StartDialogue(Dialogue dialogue) {
        currentDialogue = dialogue;
        currentPage = 0;
        typer.Type(CurrentText);
    }

    /// <summary>
    ///     Advance the dialogue to the next page.
    /// </summary>
    private void NextPage() {
        currentPage++;
        if (currentPage >= currentDialogue.pages.Length) {
            Close();
            return;
        }

        typer.Type(CurrentText);
    }

    /// <summary>
    ///     Callback to skip the typewriting effect with dialogue.
    /// </summary>
    /// <param name="context">The input action callback context.</param>
    private void OnSubmit(InputAction.CallbackContext context) {
        if (!context.ReadValueAsButton()) return;
        if (typer.IsPrinting)
            typer.Skip();
        else
            NextPage();
    }
}