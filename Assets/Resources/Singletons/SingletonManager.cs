using UnityEngine;

/// <summary>
///     Global manager that initializes all singletons.
/// </summary>
public static class SingletonManager
{
    /// <summary>
    ///     Initialize is run on game start.
    /// </summary>
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Initialize()
    {
        CreateSingletons();
    }
    
    /// <summary>
    ///     Create instances of all singletons.
    /// </summary>
    private static void CreateSingletons()
    {
        var gameManager = GameManager.Instance;
        var uiManager = UIManager.Instance;
        var saveManager = SaveManager.Instance;
        var sceneChanger = SceneChanger.Instance;
    }
}
