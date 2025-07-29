using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
///     A wrapper around the InputAction class that manages buffered inputs.
/// </summary>
public class BufferedInputAction {
    /// <summary>
    ///     The length of time before the buffer expires.
    /// </summary>
    private readonly float inputBufferTime;

    /// <summary>
    ///     The wrapped input action instance.
    /// </summary>
    public InputAction InputAction;

    /// <summary>
    ///     The time since the last input was made.
    /// </summary>
    private float timeOfLastInput;

    public BufferedInputAction(InputAction inputAction, float inputBufferTime) {
        InputAction = inputAction;
        this.inputBufferTime = inputBufferTime;

        InputAction.performed += OnPerformed;
    }

    /// <summary>
    ///     Callback for when the input action is performed.
    /// </summary>
    /// <param name="context">The input action callback context.</param>
    private void OnPerformed(InputAction.CallbackContext context) {
        if (context.ReadValueAsButton()) timeOfLastInput = Time.time;
    }

    /// <summary>
    ///     Check whether the input is buffered.
    /// </summary>
    /// <returns>Whether the input is buffered.</returns>
    public bool IsBuffered() {
        return Time.time - timeOfLastInput < inputBufferTime;
    }
}