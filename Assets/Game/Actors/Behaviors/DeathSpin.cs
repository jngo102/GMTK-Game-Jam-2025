using UnityEngine;

[RequireComponent(typeof(DeathManager))]
[RequireComponent(typeof(ParticleSystem))]
public class DeathSpin : MonoBehaviour
{
    public float spinSpeed;
    
    private DeathManager death;
    private ParticleSystem particles;
    private ParticleSystemRenderer particleRenderer;
    public Animator animator;
    public Rigidbody2D body;
    public SpriteRenderer sprite;
    public GameObject spinObj;
    
    private bool isSpinning;
    
    private void Awake()
    {
        death = GetComponent<DeathManager>();
        death.Died.AddListener(StartDeathSpin);
        
        particles = GetComponent<ParticleSystem>();
        particleRenderer = GetComponent<ParticleSystemRenderer>();
    }

    private void Update()
    {
        if (isSpinning)
        {
            spinObj.transform.rotation = Quaternion.Euler(0, 0, transform.rotation.eulerAngles.z + spinSpeed * Time.deltaTime);
            if (!sprite.isVisible && !particleRenderer.isVisible)
            {
                Destroy(spinObj);
            }
        }
    }

    private void StartDeathSpin()
    {
        body.bodyType = RigidbodyType2D.Dynamic;
        body.gravityScale = 1;
        body.constraints = RigidbodyConstraints2D.None;
        isSpinning = true;
        var spriteColor = sprite.color;
        sprite.color = new Color(spriteColor.r, spriteColor.g, spriteColor.b, 0.5f);
        particles.Play();
    }
}
