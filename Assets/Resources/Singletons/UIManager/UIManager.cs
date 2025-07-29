using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

/// <summary>
///     Singleton that manages all the user interfaces in the game.
/// </summary>
public class UIManager : Singleton<UIManager>, ISaveable {
    /// <summary>
    ///     The canvas to parent user interfaces to.
    /// </summary>
    [SerializeField] private Canvas canvas;

    /// <summary>
    ///     All the user interface prefabs that are managed by the ui manager.
    /// </summary>
    [SerializeField] private BaseUI[] uiPrefabs;

    private UIInputActions inputActions;

    /// <summary>
    ///     User interface input actions.
    /// </summary>
    public UIInputActions.UIActions Actions => inputActions.UI;

    /// <summary>
    ///     A reference instance of player actions for input rebinding.
    /// </summary>
    public PlayerInputActions ReferencePlayerActions { get; private set; }

    /// <inheritdoc />
    public void LoadData(SaveData saveData) {
        if (string.IsNullOrEmpty(saveData.bindingOverrides)) return;

        ReferencePlayerActions.asset.LoadBindingOverridesFromJson(saveData.bindingOverrides);
    }

    /// <inheritdoc />
    public void SaveData(SaveData saveData) {
        saveData.bindingOverrides = ReferencePlayerActions.asset.SaveBindingOverridesAsJson();
    }

    /// <inheritdoc />
    protected override void OnAwake() {
        inputActions = new UIInputActions();
        inputActions.Enable();
        ReferencePlayerActions = new PlayerInputActions();
        SceneManager.activeSceneChanged += OnSceneChange;
    }

    /// <summary>
    ///     Callback for when a scene changes.
    /// </summary>
    /// <param name="lastScene">The previous scene that was changed from.</param>
    /// <param name="nextScene">The next scene that has been changed to.</param>
    private void OnSceneChange(Scene lastScene, Scene nextScene) {
        if (SceneData.IsGameplayScene(nextScene.name))
            CloseUI<PauseMenu>();
        else
            DestroyUI<PauseMenu>();
    }

    /// <summary>
    ///     Open a user interface.
    /// </summary>
    /// <typeparam name="T">The type of user interface to open.</typeparam>
    /// <returns>The user interface that was opened.</returns>
    public T OpenUI<T>() where T : BaseUI {
        var ui = GetUI<T>();
        if (ui == null) return null;
        ui.Open();
        return ui;
    }

    /// <summary>
    ///     Close a user interface.
    /// </summary>
    /// <typeparam name="T">The type of user interface to close.</typeparam>
    /// <returns>Whether the user interace was successfully closed.</returns>
    public bool CloseUI<T>() where T : BaseUI {
        var ui = GetUI<T>();
        if (ui == null) return false;
        ui.Close();
        return true;
    }

    /// <summary>
    ///     Get a user interface.
    /// </summary>
    /// <typeparam name="T">The type of user interface to get.</typeparam>
    /// <returns>The user interface of type T.</returns>
    public T GetUI<T>() where T : BaseUI {
        foreach (Transform child in canvas.transform)
            if (child.TryGetComponent<T>(out var uiComponent))
                return uiComponent;

        return InstantiateUI<T>();
    }

    /// <summary>
    ///     Instantiate a user interface.
    /// </summary>
    /// <typeparam name="T">The type of user interface to instantiate.</typeparam>
    /// <returns>The user interface of type T, or null if not found.</returns>
    public T InstantiateUI<T>() where T : BaseUI {
        foreach (var ui in uiPrefabs)
            if (ui is T) {
                var uiInstance = Instantiate(ui, canvas.transform);
                return (T)uiInstance;
            }

        return null;
    }

    /// <summary>
    ///     Destroy a user interface.
    /// </summary>
    /// <typeparam name="T">The type of user interface to destroy.</typeparam>
    public void DestroyUI<T>() where T : BaseUI {
        foreach (Transform child in canvas.transform)
            if (child.TryGetComponent<T>(out var _))
                Destroy(child.gameObject);
    }
}