using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
///     Component for the game's main menu screen.
/// </summary>
public class MainMenu : MonoBehaviour {
    /// <summary>
    ///     The main home page of the main menu.
    /// </summary>
    [SerializeField] private RectTransform homePage;

    /// <summary>
    ///     Warning asking the player if they really want to exit the game.
    /// </summary>
    [SerializeField] private RectTransform exitWarning;

    /// <summary>
    ///     The button that exits from the game.
    /// </summary>
    [SerializeField] private Button exitButton;

    /// <summary>
    ///     A stack that keeps track of the pages that the player has visited.
    /// </summary>
    private readonly Stack<RectTransform> menuStack = new();

    private void Start() {
        if (Application.platform == RuntimePlatform.WebGLPlayer) exitButton.gameObject.SetActive(false);

        // Initially add home page to stack
        menuStack.Push(homePage);
    }

    private void OnEnable() {
        UIManager.Instance.Actions.Cancel.performed += _ => Back();
    }

    public void StartGame()
    {
        SceneChanger.Instance.ChangeScene("Arena");
    }

    /// <summary>
    ///     Open a menu, hiding the current one.
    /// </summary>
    /// <param name="menu">The menu to show.</param>
    public void OpenMenu(RectTransform menu) {
        var topMenu = menuStack.Peek();
        topMenu.gameObject.SetActive(false);
        menu.gameObject.SetActive(true);
        menuStack.Push(menu);
    }

    /// <summary>
    ///     Go to the previous menu screen.
    /// </summary>
    public void Back() {
        if (menuStack.Count > 1) {
            menuStack.Pop().gameObject.SetActive(false);
            var topMenu = menuStack.Peek();
            topMenu.gameObject.SetActive(true);
        }
    }

    /// <summary>
    ///     Exit the game from the main menu.
    /// </summary>
    public void ExitGame() {
        GameManager.Instance.ExitGame();
    }
}