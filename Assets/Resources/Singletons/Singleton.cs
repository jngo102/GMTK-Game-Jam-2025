using JetBrains.Annotations;
using UnityEngine;

/// <summary>
///     The base singleton class.
/// </summary>
public abstract class Singleton : MonoBehaviour {
    /// <summary>
    ///     The immediate parent directory of all the singleton prefabs in the Resources folder.
    /// </summary>
    protected const string SingletonsDirName = "Singletons";

    /// <summary>
    ///     Whether the game is quitting.
    /// </summary>
    protected static bool Quitting { get; private set; }

    private void OnApplicationQuit() {
        Quitting = true;
    }
}

/// <summary>
///     Extended singleton class.
/// </summary>
/// <typeparam name="T">The type of the singleton.</typeparam>
public abstract class Singleton<T> : Singleton where T : MonoBehaviour {
    [CanBeNull] private static T _instance;
    [NotNull] private static readonly object Lock = new();
    [SerializeField] private bool persistent = true;

    /// <summary>
    ///     The singleton instance.
    /// </summary>
    [NotNull]
    public static T Instance {
        get {
            if (Quitting) {
                Debug.LogWarning(
                    $"[{nameof(Singleton)}<{typeof(T)}>] Instance will not be returned because the application is quitting.");
                // ReSharper disable once AssignNullToNotNullAttribute
                return null;
            }

            lock (Lock) {
                if (_instance) return _instance;
                var instances = FindObjectsByType<T>(FindObjectsSortMode.None);
                var count = instances.Length;
                if (count > 0) {
                    if (count == 1) return _instance = instances[0];
                    Debug.LogWarning(
                        $"[{typeof(T)}] There should never be more than one {nameof(Singleton)} of type {typeof(T)} in the scene, but {count} were found. The first instance found will be used, and all others will be destroyed.");
                    for (var i = 1; i < instances.Length; i++) Destroy(instances[i]);
                    return _instance = instances[0];
                }

                var singletonName = typeof(T).FullName;
                var singletonPath = $"{SingletonsDirName}/{singletonName}/{singletonName}";
                Debug.Log(
                    $"[{typeof(T)}] An instance is needed in the scene and no existing instances were found, so a new instance will be created at {singletonPath}");
                return _instance = Instantiate(Resources.Load<GameObject>(singletonPath))
                    .GetComponent<T>();
            }
        }
    }

    private void Awake() {
        if (persistent) DontDestroyOnLoad(gameObject);
        OnAwake();
    }

    /// <summary>
    ///     Virtual method that is called in Awake.
    /// </summary>
    protected virtual void OnAwake() { }
}