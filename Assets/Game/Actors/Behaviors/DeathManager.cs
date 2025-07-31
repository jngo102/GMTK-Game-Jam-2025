using UnityEngine;

/// <summary>
///     Handles an actor's death.
/// </summary>
[RequireComponent(typeof(Health))]
public class DeathManager : MonoBehaviour {
    public delegate void OnDeath();

    /// <summary>
    ///     The corpse object to create once the actor has died.
    /// </summary>
    [SerializeField] private GameObject corpsePrefab;

    private Health health;

    /// <summary>
    ///     Whether the actor is dead.
    /// </summary>
    public bool IsDead { get; private set; }

    private void Awake() {
        health = GetComponent<Health>();
        // Check whether the actor should be dead every time it takes damage.
        health.Harmed += CheckDead;
    }

    /// <summary>
    ///     Raised when the actor has died.
    /// </summary>
    public event OnDeath Died;

    /// <summary>
    ///     Check whether the actor's health has reached zero.
    /// </summary>
    /// <param name="damageAmount">The amount of damage taken.</param>
    /// <param name="damageSource">The source of the damage.</param>
    private void CheckDead(float damageAmount, Damager damageSource) {
        if (health.CurrentHealth <= 0) {
            Died?.Invoke();
            Die(damageSource);
        }
    }

    /// <summary>
    ///     Called when the actor dies.
    /// </summary>
    /// <param name="damageSource">The source of the damager that killed the actor.</param>
    private void Die(Damager damageSource) {
        IsDead = true;
        if (corpsePrefab != null) {
            var corpse = Instantiate(corpsePrefab, transform.position, Quaternion.identity);
            corpse.GetComponent<Facer>().FaceObject(damageSource.transform);
        }

        Destroy(gameObject);
    }

    /// <summary>
    ///     Bring the actor back to life and fully heal it.
    /// </summary>
    public void Revive() {
        IsDead = false;
        health.FullHeal();
    }
}