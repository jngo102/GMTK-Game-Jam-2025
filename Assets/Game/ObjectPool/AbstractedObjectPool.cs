using System;
using UnityEngine;
using UnityEngine.Pool;

/// <summary>
///     A wrapper around Unity's ObjectPool class.
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class AbstractedObjectPool<T> : MonoBehaviour where T : MonoBehaviour, ISpawnable {
    /// <summary>
    ///     The object pool instance that this class is a wrapper for.
    /// </summary>
    private ObjectPool<T> pool;

    /// <summary>
    ///     The object to spawn from the pool.
    /// </summary>
    private T spawnedPrefab;

    /// <summary>
    ///     The object pool instance.
    /// </summary>
    private ObjectPool<T> Pool {
        get {
            if (pool == null)
                throw new InvalidOperationException("You need to call InitPool before using the object pool.");

            return pool;
        }
        set => pool = value;
    }

    /// <summary>
    ///     Initialize the object pool.
    /// </summary>
    /// <param name="prefab">The prefab to spawn.</param>
    /// <param name="initialSize">The initial size of the object pool.</param>
    /// <param name="maxSize">The maximum number of objects that may be in the pool.</param>
    /// <param name="collectionChecks">Whether to perform collection checks.</param>
    protected void InitPool(T prefab, int initialSize = 10, int maxSize = 20, bool collectionChecks = false) {
        spawnedPrefab = prefab;
        Pool = new ObjectPool<T>(OnCreate, OnSpawn, OnDespawn, OnDelete, collectionChecks, initialSize, maxSize);
    }

    #region Overrides

    /// <summary>
    ///     Callback for when an object is created.
    /// </summary>
    /// <returns>The newly created object.</returns>
    protected virtual T OnCreate() {
        spawnedPrefab.OnCreate();
        return Instantiate(spawnedPrefab);
    }

    /// <summary>
    ///     Callback for when an object is spawned from the pool.
    /// </summary>
    /// <param name="obj">The object that is being spawned.</param>
    protected virtual void OnSpawn(T obj) {
        obj.gameObject.SetActive(true);
        obj.OnSpawn();
    }

    /// <summary>
    ///     Callback for when an object is returned to the pool.
    /// </summary>
    /// <param name="obj">The object that is being despawned.</param>
    protected virtual void OnDespawn(T obj) {
        obj.gameObject.SetActive(false);
        obj.OnDespawn();
    }

    /// <summary>
    ///     Callback for when an object is deleted.
    /// </summary>
    /// <param name="obj">The object that is being deleted.</param>
    protected virtual void OnDelete(T obj) {
        obj.OnDelete();
        Destroy(obj);
    }

    #endregion

    #region Getters

    /// <summary>
    ///     Spawn an object.
    /// </summary>
    /// <returns>The spawned object retrieved from the pool.</returns>
    public T Spawn() {
        return Pool.Get();
    }

    /// <summary>
    ///     Despawn an object.
    /// </summary>
    /// <param name="obj">The object to despawn.</param>
    public void Despawn(T obj) {
        Pool.Release(obj);
    }

    #endregion
}