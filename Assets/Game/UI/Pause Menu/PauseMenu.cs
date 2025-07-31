using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

/// <summary>
///     Controller for the pause menu user interface.
/// </summary>
public class PauseMenu : BaseUI {
    [SerializeField] private VerticalLayoutGroup menuButtons;
    [SerializeField] private VerticalLayoutGroup exitConfirmation;

    private void Awake() {
        if (!UIManager.Instance) return;
        UIManager.Instance.Actions.Cancel.performed += OnCancel;
    }

    private void OnDestroy() {
        if (!UIManager.Instance) return;
        UIManager.Instance.Actions.Cancel.performed -= OnCancel;
    }

    /// <summary>
    ///     Callback to toggle the pause menu.
    /// </summary>
    /// <param name="context"></param>
    private void OnCancel(InputAction.CallbackContext context) {
        if (!SceneData.IsGameplayScene()) return;
        CloseExitConfirmation();
        Toggle();
    }

    /// <inheritdoc />
    public override void Open() {
        base.Open();
        GameManager.Instance.PauseGame();
    }

    /// <inheritdoc />
    public override void Close() {
        base.Close();
        GameManager.Instance.ResumeGame();
    }

    /// <summary>
    ///     Ask whether the player actually wants to exit the game.
    /// </summary>
    public void AskForExitConfirmation() {
        menuButtons.gameObject.SetActive(false);
        exitConfirmation.gameObject.SetActive(true);
    }

    /// <summary>
    ///     Close the exit confirmation popup.
    /// </summary>
    public void CloseExitConfirmation() {
        exitConfirmation.gameObject.SetActive(false);
        menuButtons.gameObject.SetActive(true);
    }

    /// <summary>
    ///     Exit to the main menu.
    /// </summary>
    public void ExitToMainMenu() {
        CloseExitConfirmation();
        Close();
        GameManager.Instance.ChangeScene("MainMenu");//, SceneTransitionType.MainMenu);
    }
}