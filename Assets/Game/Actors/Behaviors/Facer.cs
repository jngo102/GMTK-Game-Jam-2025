using System;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
///     Deals with facing an actor towards a target and flipping their scale.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class Facer : MonoBehaviour {
    public UnityAction Flipped;
    
    private Rigidbody2D body;

    private void Awake() {
        body = GetComponent<Rigidbody2D>();
    }

    /// <summary>
    ///     Face a target object.
    /// </summary>
    /// <param name="target">The target to face.</param>
    public void FaceObject(Transform target) {
        if ((target.position.x > transform.position.x && transform.localScale.x < 0) ||
            (target.position.x < transform.position.x && transform.localScale.x > 0))
            Flip();
    }

    /// <summary>
    ///     Check whether the actor should be flipped.
    /// </summary>
    public void CheckFlip() {
        if ((body.linearVelocity.x > 0 && transform.localScale.x < 0) ||
            (body.linearVelocity.x < 0 && transform.localScale.x > 0))
            Flip();
    }

    /// <summary>
    ///     Flip the actor's horizontal scale.
    /// </summary>
    public void Flip() {
        var selfTransform = transform;
        var localScale = selfTransform.localScale;
        localScale.x *= -1;
        selfTransform.localScale = localScale;
        Flipped?.Invoke();
    }
}