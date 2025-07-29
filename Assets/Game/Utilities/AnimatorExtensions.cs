using UnityEngine;

/// <summary>
///     Extensions for the Unity Animator component.
/// </summary>
public static class AnimatorExtensions {
    /// <summary>
    /// Whether an animator is currently playing an animation.
    /// </summary>
    /// <param name="animator">The animator to check.</param>
    /// <returns>Whether the animator is playing any animation.</returns>
    public static bool IsPlaying(this Animator animator)
    {
        var state = animator.GetCurrentAnimatorStateInfo(0);
        return state.length > state.normalizedTime;
    }
    
    /// <summary>
    /// Whether a certain animation on an animator is playing.
    /// </summary>
    /// <param name="animator">The animator to check.</param>
    /// <param name="animationName">The name of the animation state.</param>
    /// <returns>Whether the animator is playing the animation.</returns>
    public static bool IsPlaying(this Animator animator, string animationName)
    {
        return animator.IsPlaying() && animator.GetCurrentAnimatorStateInfo(0).IsName(animationName);
    }
    
    /// <summary>
    ///     Whether the animator has finished playing.
    /// </summary>
    /// <param name="animator">The animator to check.</param>
    /// <returns>Whether the animator has finished playing.</returns>
    public static bool IsFinished(this Animator animator) {
        return animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1;
    }
}