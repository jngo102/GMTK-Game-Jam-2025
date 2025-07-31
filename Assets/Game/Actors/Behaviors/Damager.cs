using UnityEngine;

/// <summary>
///     Deals damage to an actor.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class Damager : MonoBehaviour {
    /// <summary>
    ///     The amount of damage to deal.
    /// </summary>
    [SerializeField] private float damageAmount = 1;

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.TryGetComponent<Health>(out var healthManager))
        {
            healthManager.Hurt(damageAmount, this);
        }
    }
}