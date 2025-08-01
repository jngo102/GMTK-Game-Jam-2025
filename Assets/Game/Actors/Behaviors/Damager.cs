using FMOD.Studio;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
///     Deals damage to an actor.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class Damager : MonoBehaviour {
    /// <summary>
    ///     The amount of damage to deal.
    /// </summary>
    [SerializeField] public float damageAmount = 1;
    
    public UnityAction<Health, float> Damaged;
    
    private void OnTriggerEnter2D(Collider2D other) {
        if (other.TryGetComponent<Health>(out var health))
        {
            Damaged?.Invoke(health, damageAmount);
        }
    }
}