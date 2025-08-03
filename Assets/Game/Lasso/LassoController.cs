using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

[RequireComponent(typeof(LineRenderer))]
[RequireComponent(typeof(CircleCollider2D))]
[RequireComponent(typeof(PolygonCollider2D))]
public class LassoController : MonoBehaviour
{
    [SerializeField] private GameObject lassoPrefab;

    public float maxDrawLength = 32;
    public float lineClosedThreshold = 0.5f;

    [SerializeField] private int pointSamples = 128;

    public float lassoShrinkRadiusMultiplier = 1;

    public float lassoShrinkTimeFactor = 0.35f;

    public UnityEvent LineClosed;

    public Transform mouseTracker;

    public LassoSpinner spinner;

    private CircleCollider2D circleCollider;

    private PolygonCollider2D polyCollider;

    private Vector2[] startPoints;

    private Lasso lasso;

    private LineRenderer line;

    private bool drawingLasso;

    private bool isLassoing;

    private float shrinkTimer = Mathf.Infinity;

    private float lineLength;

    private Transform player;

    private Camera mainCam;

    public UnityEvent FirstLassoed;

    private bool firstLassoed;

    private float ShrinkTime => lineLength * lassoShrinkTimeFactor;

    public List<Lassoable> LassoedTargets { get; } = new();

    private Vector2 LassoCenter => polyCollider.bounds.center;

    private float lassoShrinkTargetRadius;
    
    private Vector2[] LassoShrinkTargetPoints =>
        startPoints.Select(point => LassoCenter + (point - LassoCenter).normalized *
            lassoShrinkTargetRadius).ToArray();

    private bool MouseButtonHeld => Mouse.current.leftButton.isPressed;

    private Vector2 MousePosition => Mouse.current.position.value;

    private void Awake()
    {
        line = GetComponent<LineRenderer>();
        circleCollider = GetComponent<CircleCollider2D>();
        polyCollider =  GetComponent<PolygonCollider2D>();
    }

    private void Start()
    {
        mainCam = Camera.main;
        
        player = GameObject.FindGameObjectWithTag("Player").transform;
        mouseTracker.position = player.position;
    }

    void Update()
    {
        var mouseWorldPos = mainCam.ScreenToWorldPoint(MousePosition);
        var newMouseTrackerPos = mouseWorldPos;
        var offset = newMouseTrackerPos - player.position;
        if (offset.magnitude > 4 * spinner.maxLassoDistance)
        {
            var angle = Mathf.Atan2(offset.y, offset.x);
            newMouseTrackerPos = player.position + new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * spinner.maxLassoDistance;
        } 
        if (mouseTracker)
        {
            mouseTracker.position = Vector3.Lerp(mouseTracker.position, newMouseTrackerPos, 4 * Time.deltaTime);
        }

        if (MouseButtonHeld && !drawingLasso && !spinner.isSpinning && !spinner.isThrowing)
        {
            StartDrawingLasso();
        }
        else if (MouseButtonHeld && drawingLasso)
        {
            var linePositionCount = line.positionCount;
            var positions = new Vector3[linePositionCount];
            line.GetPositions(positions);
            lineLength = 0;
            for (var i = 1; i < linePositionCount; i++)
            {
                var pointToPoint = (positions[i] - positions[i - 1]).magnitude;
                lineLength += pointToPoint;
            }

            if (lineLength > maxDrawLength)
            {
                line.startColor = line.endColor = Color.red;
                return;
            }

            var position = mouseWorldPos;
            position.z = 0;
            line.positionCount++;
            line.SetPosition(line.positionCount - 1, position);
        }
        else if (!MouseButtonHeld && drawingLasso)
        {
            CheckLineClosed();
        }
        else if (!drawingLasso && isLassoing && lasso)
        {
            shrinkTimer += Time.deltaTime;
            var lassoPoints = lasso.edgeCollider.pointCount;
            var newPoints = new Vector2[lassoPoints];
            for (var i = 0; i < lassoPoints; i++)
            {
                newPoints[i] = Vector2.Lerp(startPoints[i], LassoShrinkTargetPoints[i],
                    shrinkTimer / ShrinkTime);
            }

            lasso.edgeCollider.SetPoints(newPoints.ToList());
            lasso.line.SetPositions(newPoints.Select(point => new Vector3(point.x, point.y, 0)).ToArray());

            if (shrinkTimer > ShrinkTime)
            {
                isLassoing = false;
                ParentToSpinner();
            }
        }
    }

    private void StartDrawingLasso()
    {
        line.startColor = line.endColor = Color.white;

        isLassoing = false;
        drawingLasso = true;
        line.loop = false;
        if (lasso)
        {
            Destroy(lasso.gameObject);
        }

        LassoedTargets.Clear();
        ClearLine();
    }

    private void CheckLineClosed()
    {
        if (lineLength > maxDrawLength || line.positionCount <= 0)
        {
            ClearLine();
            drawingLasso = false;
            return;
        }
        
        shrinkTimer = 0;

        startPoints = new Vector2[pointSamples];
        for (int i = 0; i < pointSamples; i++)
        {
            startPoints[i] = line.GetPosition(i * (line.positionCount - 1) / pointSamples);
        }
        
        var linePositionCount = line.positionCount;
        var points = new Vector3[linePositionCount];
        line.GetPositions(points);

        polyCollider.points = startPoints;
        var polySize = polyCollider.bounds.size;
        var lassoablesArea = polySize.x * polySize.y;

        if (lassoablesArea <= 0)
        {
            ClearLine();
            return;
        }

        lassoShrinkTargetRadius = lassoablesArea * lassoShrinkRadiusMultiplier;

        if (line.positionCount > 0 && lineLength > lineClosedThreshold &&
            Vector2.Distance(line.GetPosition(0), line.GetPosition(line.positionCount - 1)) <= lineClosedThreshold)
        {
            StartLassoing();
        }

        ClearLine();
    }

    private void StartLassoing()
    {
        LineClosed?.Invoke();

        FMODUnity.RuntimeManager.PlayOneShot("event:/player/PLassoGrab");

        isLassoing = true;
        line.loop = true;

        CreateLasso();
    }

    private void CreateLasso()
    {
        var lassoObj = Instantiate(lassoPrefab);
        lasso = lassoObj.GetComponent<Lasso>();
        
        lasso.edgeCollider.SetPoints(startPoints.ToList());
        lasso.line.positionCount = pointSamples;
        var startPointsVector3 = startPoints.Select(point => new Vector3(point.x, point.y, 0)).ToArray();
        lasso.line.SetPositions(startPointsVector3);
    }

    private void ClearLine()
    {
        line.positionCount = 0;
        line.SetPositions(Array.Empty<Vector3>());
    }

    private void ParentToSpinner()
    {
        if (spinner && lasso)
        {
            circleCollider.offset = LassoCenter;
            circleCollider.radius = lassoShrinkTargetRadius;
            foreach (var lassoable in FindObjectsByType<Lassoable>(FindObjectsSortMode.None))
            {
                if (circleCollider.OverlapPoint(lassoable.transform.position))
                {
                    LassoedTargets.Add(lassoable);
                    lassoable.GetLassoed();
                }
            }

            line.positionCount = 0;
            var emptyPositions = Array.Empty<Vector3>();
            line.SetPositions(emptyPositions);
            if (LassoedTargets.Count <= 0)
            {
                Destroy(lasso.gameObject);
                return;
            }

            if (!firstLassoed)
            {
                firstLassoed = true;
                FirstLassoed?.Invoke();   
            }
            
            lasso.lassoed = LassoedTargets;
            lasso.EnableDamager();
            lasso.RepositionCenter();
            lasso.edgeCollider.enabled = false;
            spinner.StartSpinning(lasso, LassoedTargets);
        }
    }
}