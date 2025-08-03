
using System.Collections;
using UnityEngine;

/// <summary>
///     Controller for the fader user interface.
/// </summary>
[RequireComponent(typeof(Animator))]
public class Fader : BaseUI {
    private Animator animator;

    private void Awake() {
        animator = GetComponent<Animator>();
    }

    /// <inheritdoc />
    public override void Open() {
        StartCoroutine(FadeIn());
    }

    /// <inheritdoc />
    public override void Close() {
        StartCoroutine(FadeOut());
    }

    /// <summary>
    ///     Fade in to a black screen.
    /// </summary>
    public IEnumerator FadeIn() {
        base.Open();
        animator.Play("Fade In");
        yield return new WaitUntil(() => animator.IsFinished());
    }

    /// <summary>
    ///     Fade out from a black screen.
    /// </summary>
    /// <returns></returns>
    public IEnumerator FadeOut() {
        animator.Play("Fade Out");
        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1);
        yield return new WaitUntil(() => animator.IsFinished());
        base.Close();
    }
}