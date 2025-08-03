using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

[RequireComponent(typeof(LineRenderer))]
public class LassoSpinner : MonoBehaviour
{
    public Transform anchor;

    public float mouseTrackSpeedMultiplier = 8;

    public float maxLassoDistance = 4;

    public float velocityMultiplier = 32;    
    
    public float throwMinForce = 5;

    public GameObject player;
    private Animator playerAnim;
    private SpriteRenderer playerSprite;
    private Facer playerFacer;
    private DeathManager playerDeath;
    public Sprite playerSpinUpSprite;
    public Sprite playerSpinDownSprite;
    public Sprite playerSpinLeftSprite;
    public Sprite playerSpinRightSprite;


    public Lasso lasso;

    private List<Lassoable> lassoed = new();
    private List<Vector3> lassoedOffsets;

    private Collider2D lassoDamageCollider;

    private float TotalMass => lassoed.Sum(target => target.body.mass);

    public bool isThrowing;

    private LineRenderer line;

    [Header("Audio")] public string fmodSpinEvent;

    public float spinTimeScale = 4;
    public float spinTimeLogFactor = 10;
    private float spinEventTime;
    private float spinTimer;

    public bool isSpinning;

    private Vector3 previousLassoPosition;

    private float mouseVelocity;

    public UnityEvent FirstThrow;
    
    private bool firstThrow;

    private Vector3 CurrentMousePosition
    {
        get
        {
            var mainCam = Camera.main;
            if (mainCam)
            {
                var worldPoint = mainCam.ScreenToWorldPoint(Mouse.current.position.value);
                return new Vector3(worldPoint.x, worldPoint.y, 0);
            }

            return Vector3.zero;
        }
    }

    private void Awake()
    {
        line = GetComponent<LineRenderer>();

        playerAnim = player.GetComponent<Animator>();
        playerSprite = player.GetComponent<SpriteRenderer>();
        playerFacer = player.GetComponent<Facer>();
        playerDeath = player.GetComponentInChildren<DeathManager>();

        playerDeath.Died.AddListener(StopSpinning);
    }

    private void LateUpdate()
    {
        if (!lasso && isSpinning && !isThrowing)
        {
            StopSpinning();
        }
        else if (isSpinning && !isThrowing)
        {
            line.SetPosition(0, anchor.position);

            var anchorToMouse = CurrentMousePosition - anchor.position;
            var anchorToMouseAngle = Mathf.Atan2(anchorToMouse.y, anchorToMouse.x);
            var targetLassoVectorLength = Mathf.Min(anchorToMouse.magnitude, maxLassoDistance);
            var targetLassoPosition = anchor.position +
                                      new Vector3(Mathf.Cos(anchorToMouseAngle), Mathf.Sin(anchorToMouseAngle), 0) *
                                      targetLassoVectorLength;
            var nextLassoPosition = Vector3.Lerp(lasso.transform.position, targetLassoPosition,
                mouseTrackSpeedMultiplier * Time.deltaTime / TotalMass);

            var velocity = (nextLassoPosition - previousLassoPosition) * Time.deltaTime;
            lasso.velocity = velocityMultiplier * velocity;
            lasso.transform.position = nextLassoPosition;

            line.SetPosition(1, nextLassoPosition);
            for (var i = 0; i < lassoed.Count; i++)
            {
                if (!lassoed[i])
                {
                    continue;
                }

                lassoed[i].transform.position =
                    new Vector3(nextLassoPosition.x, nextLassoPosition.y, 0) + lassoedOffsets[i];
            }

            UpdateThrow();

            UpdatePlayerSprite(anchorToMouseAngle);

            UpdateSpinSound(velocity.magnitude);
        }
        else if (isThrowing)
        {
            isThrowing = false;
        }
    }

    private void FixedUpdate()
    {
        if (lasso && (previousLassoPosition - lasso.transform.position).magnitude > Mathf.Epsilon)
        {
            previousLassoPosition = lasso.transform.position;
        }
    }


    public void StartSpinning(Lasso lasso, List<Lassoable> lassoed)
    {
        playerAnim.enabled = false;

        previousLassoPosition = lasso.transform.position;
        lasso.transform.SetParent(transform, true);

        line.positionCount = 2;
        line.SetPosition(0, anchor.position);
        line.SetPosition(1, lasso.transform.position);
        isSpinning = true;
        this.lasso = lasso;
        lassoDamageCollider = this.lasso.damageCollider;
        foreach (var target in lassoed)
        {
            if (target.death.IsDead)
            {
                target.LassoReleased();
                continue;
            }

            Physics2D.IgnoreCollision(lassoDamageCollider, target.health.collider);
            this.lassoed.Add(target);
            target.death.Died.AddListener(() =>
            {
                var index = this.lassoed.IndexOf(target);
                if (index >= 0)
                {
                    this.lassoed.RemoveAt(index);
                    lassoedOffsets.RemoveAt(index);
                }

                if (this.lassoed.Count <= 0)
                {
                    StopSpinning();
                }
            });
        }

        lassoedOffsets = this.lassoed.Select(target => target.transform.position - lasso.transform.position).ToList();

        lasso.lassoed = new List<Lassoable>(this.lassoed);

        var numLassoed = this.lassoed.Count;
        lasso.line.positionCount = numLassoed + 1;
        lasso.line.SetPosition(0, Vector3.zero);
        for (var i = 0; i < numLassoed; i++)
        {
            var lassoedPos = this.lassoed[i].transform.position - lasso.transform.position;
            lasso.line.SetPosition(i + 1, lassoedPos);
        }
    }

    public void StopSpinning()
    {
        isSpinning = false;
        line.positionCount = 0;
        line.SetPositions(Array.Empty<Vector3>());
        playerAnim.enabled = true;
        playerFacer.enabled = true;
        foreach (var target in lassoed)
        {
            Physics2D.IgnoreCollision(lassoDamageCollider, target.health.collider, false);
            target.LassoReleased();
        }

        lassoed.Clear();
        if (lasso)
        {
            Destroy(lasso.gameObject);   
        }
    }

    private void UpdateThrow()
    {
        if (Mouse.current.leftButton.isPressed && !isThrowing)
        {
            if (lasso.velocity.magnitude * lassoed.Count >= throwMinForce)
            {
                ThrowLassoed();
            }
            else
            {
                isThrowing = false;
                StopSpinning();
            }
        }
    }

    private void ThrowLassoed()
    {
        if (!firstThrow)
        {
            firstThrow = true;
            FirstThrow?.Invoke();   
        }
        
        isThrowing = true;
        if (lasso)
        {
            Destroy(lasso.gameObject);   
        }
        foreach (var target in lassoed)
        {
            foreach (var other in lassoed)
            {
                Physics2D.IgnoreCollision(other.lassoThrowDamager.damageCollider, target.lassoThrowDamager.damageCollider);
            }
            var velocity = lasso.velocity;
            var speed = velocity.magnitude;
            var angle = Mathf.Atan2(velocity.y, velocity.x);
            angle += Random.Range(-Mathf.PI / 12, Mathf.PI / 12);
            velocity = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * speed;
            target.Throw(velocity);
        }
        isSpinning = false;
        line.positionCount = 0;
        line.SetPositions(Array.Empty<Vector3>());
        playerAnim.enabled = true;
        playerFacer.enabled = true;
        lassoed.Clear();
    }

    private void UpdatePlayerSprite(float angle)
    {
        if (playerSprite)
        {
            playerFacer.enabled = false;
            playerSprite.transform.localScale = Vector3.one;
            var angleDeg = angle * Mathf.Rad2Deg;
            if (angleDeg is > -45 and <= 0 or > 0 and <= 45)
            {
                playerSprite.sprite = playerSpinRightSprite;
                lasso.line.sortingOrder = 0;
            }
            else if (angleDeg is > 45 and <= 135)
            {
                playerSprite.sprite = playerSpinUpSprite;
                lasso.line.sortingOrder = 0;
            }
            else if (angleDeg is > 135 and <= 180 or > -180 and <= -135)
            {
                playerSprite.sprite = playerSpinLeftSprite;
                lasso.line.sortingOrder = 0;
            }
            else if (angleDeg is > -135 and < -45)
            {
                playerSprite.sprite = playerSpinDownSprite;
                lasso.line.sortingOrder = 100;
            }
        }
    }

    private void UpdateSpinSound(float lassoSpeed)
    {
        if (!string.IsNullOrEmpty(fmodSpinEvent) && playerSprite)
        {
            spinEventTime = Mathf.Log(spinTimeScale * lassoed.Count / lassoSpeed, spinTimeLogFactor);
            spinTimer += Time.deltaTime;
            if (spinTimer >= spinEventTime)
            {
                spinTimer = 0;

                FMODUnity.RuntimeManager.StudioSystem.setParameterByName("Force", TotalMass);
                FMODUnity.RuntimeManager.PlayOneShot(fmodSpinEvent, playerSprite.transform.position);
            }
        }
    }
}