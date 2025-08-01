using UnityEngine;

public class Lassoable : MonoBehaviour
{
    public Animator animator;
    public Collider2D Collider { get; private set; }

    public DeathManager death;

    public Mover mover;

    public Health health;

    public Rigidbody2D body;
    
    private bool gettingLassoed;

    private Vector2 targetPosition;

    private LassoController lasso;

    private void Awake()
    {
        Collider = GetComponent<Collider2D>();
    }

    public void GetLassoed()
    {
        gettingLassoed = true;
        health.enabled = false;
        mover.enabled = false;
    }

    public void LassoReleased()
    {
        gettingLassoed = false;
        health.enabled = true;
        mover.enabled = true;
    }
}