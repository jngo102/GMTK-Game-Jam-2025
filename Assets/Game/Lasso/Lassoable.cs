using System;
using UnityEngine;
using UnityEngine.Events;

public class Lassoable : MonoBehaviour
{
    public Animator animator;

    public SpriteRenderer sprite;
    public Collider2D Collider { get; private set; }
    
    public DeathManager death;
    
    public Damager lassoThrowDamager;

    public bool isThrown;

    public UnityEvent Thrown;
    
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
        Physics2D.IgnoreCollision(lassoThrowDamager.damageCollider, health.collider);
        lassoThrowDamager.Damaged.AddListener((otherHealth, _) =>
        {
            var lassoable = otherHealth.GetComponentInParent<Lassoable>();
            if (lassoable && lassoable.isThrown)
            {
                return;
            }
            animator.enabled = true;
            lassoThrowDamager.gameObject.SetActive(false);
            otherHealth.InstantKill();
            health.InstantKill();
        });
    }

    private void Update()
    {
        if (isThrown)
        {
            if (!sprite.isVisible && !death.IsDead)
            {
                health.InstantKill();
            }
        }
    }

    public void GetLassoed()
    {
        gettingLassoed = true;
        attacker.enabled = false;
        health.SetInvincible();
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
        health.SetInvincible(false);
        mover.enabled = true;
    }

    public void Throw(Vector2 velocity)
    {
        Thrown?.Invoke();
        gettingLassoed = false;
        isThrown = true;
        body.linearVelocity = velocity;
    }
}