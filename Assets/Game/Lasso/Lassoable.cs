using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class Lassoable : MonoBehaviour
{
    public Animator animator;
    public CircleCollider2D Collider { get; private set; }

    public float weight = 1;

    public Mover mover;

    public Health health;

    public Rigidbody2D body;
    
    private bool gettingLassoed;

    private Vector2 targetPosition;

    private LassoController lasso;

    private void Awake()
    {
        Collider = GetComponent<CircleCollider2D>();
    }

    public void GetLassoed()
    {
        health.enabled = false;
        gettingLassoed = true;
        if (mover)
        {
            mover.enabled = false;
        }
    }
}