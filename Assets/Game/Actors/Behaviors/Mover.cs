using System.Collections;
using UnityEngine;

/// <summary>
///     Manages horizontal movement for the actor.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Facer))]
public class Mover : MonoBehaviour {
    /// <summary>
    ///     The speed at which the actor moves.
    /// </summary>
    [SerializeField] private float moveSpeed = 5;

    private Rigidbody2D body;
    private Facer facer;

    private void Awake() {
        body = GetComponent<Rigidbody2D>();
        facer = GetComponent<Facer>();
    }

    /// <summary>
    ///     Perform movement.
    /// </summary>
    /// <param name="direction">The direction that the actor moves toward.</param>
    public void Move(Vector2 direction) {
        var velocity = direction * moveSpeed;
        body.linearVelocity = velocity;
        var velocityX = velocity.x;
        var scaleX = transform.localScale.x;
        if ((scaleX < 0 && velocityX > 0) || (scaleX > 0 && velocityX < 0))
        {
            facer.Flip();
        }
    }

    /// <summary>
    ///     Stop moving.
    /// </summary>
    public void Stop()
    {
        body.linearVelocity = Vector2.zero;
    }
}