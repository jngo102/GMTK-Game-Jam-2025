using UnityEngine;

/// <summary>
///     Behaviour for shaking a game object.
/// </summary>
public class Shaker : MonoBehaviour {
    private float currentAmplitude;
    private Vector3 originalPosition;
    private float shakeAmplitude;
    private float shakeDuration;
    private float shakeTime;
    private bool shaking;

    private void Awake() {
        UpdateOriginalPosition();
    }

    private void Update() {
        if (!shaking) {
            UpdateOriginalPosition();
        }
        else {
            transform.localPosition = originalPosition + Random.onUnitSphere * currentAmplitude;
            shakeTime += Time.deltaTime;
            currentAmplitude -= shakeAmplitude * Time.deltaTime / shakeDuration;

            if (shakeTime >= shakeDuration) StopShake();
        }
    }

    /// <summary>
    ///     Start shaking the game object.
    /// </summary>
    /// <param name="amplitude">The amount to shake the object by.</param>
    /// <param name="duration">The duration to shake the object for.</param>
    public void StartShake(float amplitude, float duration) {
        shakeTime = 0;
        shaking = true;
        currentAmplitude = shakeAmplitude = amplitude;
        shakeDuration = duration;
    }

    /// <summary>
    ///     Stop shaking the game object.
    /// </summary>
    public void StopShake() {
        shaking = false;
        transform.localPosition = originalPosition;
    }

    /// <summary>
    ///     Update the game object's original position before shaking.
    /// </summary>
    public void UpdateOriginalPosition() {
        originalPosition = transform.localPosition;
    }
}