using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
///     Controller for the fader user interface.
/// </summary>
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Image))]
public class Fader : BaseUI {
    private Animator animator;
    private Image image;

    private void Awake() {
        animator = GetComponent<Animator>();
        image = GetComponent<Image>();
    }

    private void Update()
    {
        if (animator.IsPlaying())
        {
        }
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
        yield return new WaitUntil(() => animator.IsFinished());
        base.Close();
    }
}