using UnityEngine;

public class Lassoable : MonoBehaviour
{
    public Animator animator;

    public SpriteRenderer sprite;
    public Collider2D Collider { get; private set; }
    
    public DeathManager death;

    public Sprite lassoedSprite;

    public Mover mover;

    public Health health;

    public Rigidbody2D body;

    public Attacker attacker;
    
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
        attacker.enabled = false;
        health.enabled = false;
        mover.enabled = false;
        animator.StopPlayback();
        animator.enabled = false;
        sprite.sprite = lassoedSprite;
    }

    public void LassoReleased()
    {
        animator.enabled = true;
        attacker.enabled = true;
        gettingLassoed = false;
        health.enabled = true;
        mover.enabled = true;
    }
}