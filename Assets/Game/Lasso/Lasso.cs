using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(EdgeCollider2D))]
[RequireComponent(typeof(LineRenderer))]
public class Lasso : MonoBehaviour
{
    public EdgeCollider2D edgeCollider;
    public LineRenderer line;
    public Rigidbody2D body;

    public List<Lassoable> lassoed = new();

    public float hitShakeMultiplier = 0.1f;

    public float hitStopMultiplier = 0.05f;

    public float damageSpeedMultiplier = 8;

    private int LassoedCount => lassoed.Count;

    public Damager damager;

    public Vector2 velocity;

    private float Speed => velocity.magnitude;

    private Vector2 LassoCenter => edgeCollider.bounds.center;

    private void Awake()
    {
        edgeCollider = GetComponent<EdgeCollider2D>();
        line = GetComponent<LineRenderer>();
        damager.Damaged += LassoHit;
    }

    private void Update()
    {
        damager.damageAmount = LassoedCount * damageSpeedMultiplier * Speed;
    }

    private void LassoHit(Health health, float damageAmount)
    {
        var lassoable = health.GetComponentInParent<Lassoable>();
        if (lassoable && lassoed.Contains(lassoable) || health.CompareTag("Player"))
        {
            return;
        }

        lassoable.body.AddForce(velocity, ForceMode2D.Impulse);

        UIManager.Instance.camera.shaker.StartShake(LassoedCount * hitShakeMultiplier * LassoedCount * Speed, 0.25f);
        GameManager.Instance.HitStop(hitStopMultiplier * LassoedCount * Speed);
    }

    public void RepositionCenter()
    {
        transform.localPosition = LassoCenter;
        for (var i = 0; i < edgeCollider.pointCount; i++)
        {
            var newPoints = new Vector2[edgeCollider.pointCount];
            for (var j = 0; j < edgeCollider.pointCount; j++)
            {
                newPoints[j] = edgeCollider.points[j] - LassoCenter;
            }

            edgeCollider.SetPoints(newPoints.ToList());
            line.SetPositions(newPoints.Select(point => new Vector3(point.x, point.y, 0)).ToArray());
        }
        
        UIManager.Instance.camera.AddTarget(transform);
    }
}