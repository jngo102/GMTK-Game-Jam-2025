using System.Collections.Generic;
using UnityEngine;

/// <summary>
///     A camera that follows a target.
/// </summary>
[RequireComponent(typeof(Camera))]
[RequireComponent(typeof(Shaker))]
public class CameraController : MonoBehaviour {
    [SerializeField] private float smoothing = 0.5f;
    [SerializeField] private Vector3 offset;
    [SerializeField] private float minZoom = 40;
    [SerializeField] private float maxZoom = 10;
    [SerializeField] private float zoomLimit = 50;

    private readonly List<Transform> targets = new();
    private new Camera camera;
    private Shaker shaker;
    private Vector3 velocity;
    
    private void Awake() {
        camera = GetComponent<Camera>();
        shaker = GetComponent<Shaker>();
        GameManager.Instance.LevelStarted += ResetPosition;
    }

    private void LateUpdate() {
        KeepTargetsInFrame();
    }
    
    /// <summary>
    ///     Add a target to the camera controller's list of targets to follow.
    /// </summary>
    /// <param name="target"></param>
    public void AddTarget(Transform target) {
        targets.Add(target);
    }
    
    /// <summary>
    ///     Remove a target from the camera controller's list of targets to follow.
    /// </summary>
    /// <param name="target"></param>
    public void RemoveTarget(Transform target) {
        targets.Remove(target);
    }

    /// <summary>
    ///     Reset the camera to the targets' positions.
    /// </summary>
    private void ResetPosition() {
        FollowPosition(false);
        FollowZoom(false);
    }

    /// <summary>
    ///     Keep all targets in the camera's view.
    /// </summary>
    private void KeepTargetsInFrame() {
        FollowPosition();
        FollowZoom();
    }
    
    /// <summary>
    ///     Follow targets, modifying the camera's position.
    /// </summary>
    /// <param name="smooth">Whether to smooth the position change.</param>
    private void FollowPosition(bool smooth = true) {
        if (targets.Count <= 0) return;

        var centerPoint = GetCenterPoint();
        var newPos = centerPoint + offset;
        var selfTransform = transform;
        selfTransform.position =
            smooth ? Vector3.SmoothDamp(selfTransform.position, newPos, ref velocity, smoothing) : newPos;
    }

    /// <summary>
    ///     Follow targets, modifying the camera's zoom.
    /// </summary>
    /// <param name="smooth">Whether to smooth the zoom change.</param>
    private void FollowZoom(bool smooth = true) {
        if (targets.Count <= 0) return;
        
        var newZoom = Mathf.Lerp(maxZoom, minZoom, GetMaxDistance() / zoomLimit);
        camera.fieldOfView = smooth ? Mathf.Lerp(camera.fieldOfView, newZoom, Time.deltaTime) : newZoom;
    }

    /// <summary>
    /// Get the maximum horizontal distance between all targets.
    /// </summary>
    /// <returns>The maximum horizontal distance between all targets.</returns>
    private float GetMaxDistance() {
        var bounds = new Bounds(targets[0].position, Vector3.zero);
        foreach (var target in targets) {
            bounds.Encapsulate(target.position);
        }

        return bounds.size.x;
    }

    /// <summary>
    /// Get the center point among all targets.
    /// </summary>
    /// <returns>The center point among all targets.</returns>
    private Vector3 GetCenterPoint() {
        if (targets.Count == 1) {
            return targets[0].position;
        }

        var bounds = new Bounds(targets[0].position, Vector3.zero);
        foreach (var target in targets) {
            bounds.Encapsulate(target.position);
        }

        return bounds.center;
    }
}