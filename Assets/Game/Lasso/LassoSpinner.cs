using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(LineRenderer))]
public class LassoSpinner : MonoBehaviour
{
    public Transform anchor;

    public float mouseTrackSpeedMultiplier = 8;

    public float maxLassoDistance = 4;

    public Animator playerAnim;
    public SpriteRenderer playerSprite;
    public Sprite playerSpinUpSprite;
    public Sprite playerSpinDownSprite;
    public Sprite playerSpinLeftSprite;
    public Sprite playerSpinRightSprite;

    public Lasso lasso;

    private List<Lassoable> lassoed;
    private List<Vector3> lassoedOffsets;

    private Damager LassoDamager => lasso?.GetComponent<Damager>();

    private float TotalWeight => lassoed.Sum(target => target.weight);

    private LineRenderer line;

    public float spinSpeed;

    public bool isSpinning;

    private Vector3 previousLassoPosition;

    private float mouseVelocity;

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
    }

    private void Update()
    {
        if (!lasso && isSpinning)
        {
            isSpinning = false;
            line.positionCount = 0;
            line.SetPositions(Array.Empty<Vector3>());
            playerAnim.enabled = true;
        }
        else if (isSpinning)
        {
            line.SetPosition(0, anchor.position);

            var anchorToMouse = CurrentMousePosition - anchor.position;
            var anchorToMouseAngle = Mathf.Atan2(anchorToMouse.y, anchorToMouse.x);
            var targetLassoVectorLength = Mathf.Min(anchorToMouse.magnitude, maxLassoDistance);
            var targetLassoPosition = anchor.position +
                                      new Vector3(Mathf.Cos(anchorToMouseAngle), Mathf.Sin(anchorToMouseAngle), 0) *
                                      targetLassoVectorLength;
            var nextLassoPosition = Vector3.Lerp(lasso.transform.position, targetLassoPosition,
                mouseTrackSpeedMultiplier * Time.deltaTime / TotalWeight);

            // Debug.Log("Next Lasso pos: " +  nextLassoPosition);

            lasso.velocity = nextLassoPosition - previousLassoPosition;
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

            if (playerSprite)
            {
                var scaleX = playerSprite.transform.localScale.x;
                playerSprite.sprite = anchorToMouseAngle switch
                {
                    > -Mathf.PI / 4 and < Mathf.PI / 4 when scaleX > 0 => playerSpinRightSprite,
                    > -Mathf.PI / 4 and < Mathf.PI / 4 when scaleX < 0 => playerSpinLeftSprite,
                    > Mathf.PI / 4 and < 3 * Mathf.PI / 4 => playerSpinUpSprite,
                    > 3 * Mathf.PI / 4 and < 5 * Mathf.PI / 4 when scaleX > 0 => playerSpinLeftSprite,
                    > 3 * Mathf.PI / 4 and < 5 * Mathf.PI / 4 when scaleX < 0 => playerSpinRightSprite,
                    > 5 * Mathf.PI / 4 and < 7 * Mathf.PI / 4 when scaleX > 0 => playerSpinDownSprite,

                    _ => playerSprite.sprite
                };
            }
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
        this.lassoed = lassoed;
        lassoedOffsets = lassoed.Select(target => target.transform.position - lasso.transform.position).ToList();
    }
}