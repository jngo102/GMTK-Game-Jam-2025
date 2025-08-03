using System;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public Rigidbody2D body;

    public SpriteRenderer sprite;
    public ParticleSystemRenderer particles;
    
    public float fireSpeed = 8;
    
    public void Awake()
    {
        body = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        var rotZ = transform.rotation.eulerAngles.z * Mathf.Deg2Rad;
        body.linearVelocity = new Vector2(Mathf.Cos(rotZ), Mathf.Sin(rotZ)) *  fireSpeed;
    }

    private void Update()
    {
        if (!sprite.isVisible && !particles.isVisible)
        {
            Destroy(gameObject);
        }
    }
}
