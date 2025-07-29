using System.Collections.Generic;

/// <summary>
///     Information about the scenes in the game.
/// </summary>
public static class SceneData {
    /// <summary>
    ///     All the scenes in the game that are not gameplay.
    /// </summary>
    private static readonly List<string> _nonGameplayScenes = new() {
        "MainMenu"
    };

    /// <summary>
    ///     Check whether a scene is a gameplay scene.
    /// </summary>
    /// <param name="sceneName">The name of the scene to check.</param>
    /// <returns>Whether the scene is a gameplay scene.</returns>
    public static bool IsGameplayScene(string sceneName) {
        return !_nonGameplayScenes.Contains(sceneName);
    }
}