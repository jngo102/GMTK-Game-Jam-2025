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
    public Facer playerFacer;
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

    private void LateUpdate()
    {
        if (!lasso && isSpinning)
        {
            StopSpinning();
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
                playerFacer.enabled = false;
                playerSprite.transform.localScale = Vector3.one;
                var angleDeg = anchorToMouseAngle * Mathf.Rad2Deg;
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
        this.lassoed = new List<Lassoable>(lassoed);
        lassoedOffsets = lassoed.Select(target => target.transform.position - lasso.transform.position).ToList();
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
            target.LassoReleased();
        }
        lassoed.Clear();
    }
}