using System;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CircleCollider2D))]
public class Lassoable : MonoBehaviour
{
    private Animator animator;
    public CircleCollider2D Collider { get; private set; }

    private bool gettingLassoed;

    private Vector2 targetPosition;

    private LassoController lasso;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        Collider = GetComponent<CircleCollider2D>();
    }

    private void Update()
    {
        if (gettingLassoed)
        {
            
        }
    }

    public void GetLassoed(LassoController lasso, Vector2 targetPosition)
    {
        gettingLassoed = true;
        this.lasso = lasso;
        this.targetPosition = targetPosition;
    }
}