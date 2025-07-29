/// <summary>
///     Interface for objects that are managed by an object pool.
/// </summary>
public interface ISpawnable {
    /// <summary>
    ///     Callback for when the object is created.
    /// </summary>
    void OnCreate();

    /// <summary>
    ///     Callback for when the object is spawned from the pool.
    /// </summary>
    void OnSpawn();

    /// <summary>
    ///     Callback for when the object is returned to the pool.
    /// </summary>
    void OnDespawn();

    /// <summary>
    ///     Callback for when the object is destroyed.
    /// </summary>
    void OnDelete();
}