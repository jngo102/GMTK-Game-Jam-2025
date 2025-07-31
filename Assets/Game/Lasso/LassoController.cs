using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PolygonCollider2D))]
public class LassoController : MonoBehaviour
{
    [SerializeField] private GameObject linePrefab;

    [SerializeField] private Transform player;

    public float lineClosedThreshold = 0.5f;

    [SerializeField] private int pointSamples = 128;

    public float lassoShrinkRadiusMultiplier = 1;

    public float lassoShrinkTime = 0.35f;

    public UnityEvent LineClosed;

    private EdgeCollider2D lineCollider;

    private PolygonCollider2D polyCollider;

    private Vector2[] startPoints;

    private LineRenderer line;

    private bool drawingLasso;

    private bool isLassoing;

    private float shrinkTimer = Mathf.Infinity;

    public List<Lassoable> LassoedTargets { get; } = new();

    private bool HasLassoed => LassoedTargets.Count > 0;

    private Vector2 LassoCenter => polyCollider.bounds.center;

    private float LassoablesArea => LassoedTargets.Sum(target => Mathf.PI * Mathf.Pow(target.Collider.radius, 2));
    
    public float LassoShrinkTargetRadius
    {
        get
        {
            // if (LassoedTargets.Count <= 0)
            // {
            //     return 0;
            // }
            //
            // if (LassoedTargets.Count == 1)
            // {
            //     return LassoedTargets[0].Collider.radius;
            // }
            //
            // var maxTargetX = LassoedTargets.Max(target => target.Collider.bounds.max.x);
            // var maxTargetY = LassoedTargets.Max(target => target.Collider.bounds.max.y);
            // var minTargetX = LassoedTargets.Min(target => target.Collider.bounds.min.x);
            // var minTargetY = LassoedTargets.Min(target => target.Collider.bounds.min.y);
            // var width = maxTargetX - minTargetX;
            // var height = maxTargetY - minTargetY;
            // return (width + height) / 2;
            return Mathf.Sqrt(LassoablesArea / Mathf.PI) * 1.25f;
        }
    }

    private Vector2[] LassoShrinkTargetPoints =>
        polyCollider.points.Select(point => LassoCenter + (point - LassoCenter).normalized *
            LassoShrinkTargetRadius).ToArray();

    private float AngleDiffRadians
    {
        get
        {
            if (LassoedTargets.Count <= 0)
            {
                return 0;
            }

            return 2 * Mathf.PI / LassoedTargets.Count;
        }
    }

    private Vector2 LassoedTargetsCenter => new Vector2(LassoedTargets.Sum(target => target.transform.position.x),
        LassoedTargets.Sum(target => target.transform.position.y)) / pointSamples;

    private Bounds LassoBounds
    {
        get
        {
            var maxTargetX = LassoedTargets.Max(target => target.transform.position.x);
            var maxTargetY = LassoedTargets.Max(target => target.transform.position.y);
            var minTargetX = LassoedTargets.Min(target => target.transform.position.x);
            var minTargetY = LassoedTargets.Min(target => target.transform.position.y);
            var width = maxTargetX - minTargetX;
            var height = maxTargetY - minTargetY;
            return new Bounds();
        }
    }

    private bool MouseButtonHeld => Mouse.current.leftButton.isPressed;

    private Vector2 MousePosition => Mouse.current.position.value;

    private void Awake()
    {
        polyCollider = GetComponent<PolygonCollider2D>();
    }

    void Update()
    {
        if (MouseButtonHeld && !drawingLasso)
        {
            CreateLasso();
        }
        else if (MouseButtonHeld && drawingLasso && line && Camera.main)
        {
            var position = Camera.main.ScreenToWorldPoint(MousePosition);
            position.z = 0;
            line.positionCount++;
            line.SetPosition(line.positionCount - 1, position);
        }
        else if (!MouseButtonHeld && drawingLasso && line)
        {
            CheckLineClosed();
        }
        else if (!drawingLasso && isLassoing && line && HasLassoed && shrinkTimer < lassoShrinkTime)
        {
            shrinkTimer += Time.deltaTime;
            var newPoints = new Vector2[lineCollider.pointCount];
            for (var i = 0; i < lineCollider.pointCount; i++)
            {
                newPoints[i] = Vector2.Lerp(startPoints[i], LassoShrinkTargetPoints[i],
                    shrinkTimer / lassoShrinkTime);
            }

            lineCollider.SetPoints(newPoints.ToList());
            line.SetPositions(newPoints.Select(point => new Vector3(point.x, point.y, 0)).ToArray());
        }
    }

    private void CreateLasso()
    {
        isLassoing = false;
        drawingLasso = true;
        DestroyLine();
        polyCollider.enabled = false;

        var lineObj = Instantiate(linePrefab);
        line = lineObj.GetComponent<LineRenderer>();
        lineCollider = lineObj.GetComponent<EdgeCollider2D>();
        line.positionCount = 0;
    }

    private void CheckLineClosed()
    {
        drawingLasso = false;
        var linePositionCount = line.positionCount;
        var points = new Vector3[linePositionCount];
        line.GetPositions(points);
        float lineLength = 0;
        for (var i = 1; i < linePositionCount; i++)
        {
            lineLength += Vector3.Distance(points[i], points[i - 1]);
        }

        if (line.positionCount > 0 && lineLength > lineClosedThreshold &&
            Vector2.Distance(line.GetPosition(0), line.GetPosition(line.positionCount - 1)) <=
            lineClosedThreshold)
        {
            StartLassoing();
        }
        else
        {
            DestroyLine();
        }
    }

    private void StartLassoing()
    {
        LineClosed?.Invoke();
        isLassoing = true;
        line.loop = true;
        shrinkTimer = 0;
        startPoints = new Vector2[pointSamples];
        for (int i = 0; i < pointSamples; i++)
        {
            startPoints[i] = line.GetPosition(i * (line.positionCount - 1) / pointSamples);
        }

        polyCollider.enabled = true;
        polyCollider.points = startPoints;
        foreach (var lassoable in FindObjectsByType<Lassoable>(FindObjectsSortMode.None))
        {
            if (polyCollider.OverlapPoint(lassoable.transform.position))
            {
                LassoedTargets.Add(lassoable);
            }
        }

        if (!HasLassoed)
        {
            DestroyLine();
            return;
        }
        
        var startPointsVector3 = startPoints.Select(point => new Vector3(point.x, point.y, 0)).ToArray();
        line.positionCount = pointSamples;
        line.SetPositions(startPointsVector3);
        
        lineCollider.SetPoints(startPoints.ToList());
        lineCollider.transform.localScale = Vector3.one;
        line.transform.SetParent(lineCollider.transform);
    }

    private void ClearLassoPoints()
    {
        if (!HasLassoed)
        {
            DestroyLine();
        }

        polyCollider.enabled = false;
    }

    private void DestroyLine()
    {
        Debug.Log("DESTROY LINE");
        LassoedTargets.Clear();
        if (line)
        {
            Destroy(line.gameObject);
        }
    }

    private void ParentToPlayer()
    {
        if (player)
        {
            transform.SetParent(player);
        }
    }

    private void LassoTarget(Lassoable target)
    {
    }
}