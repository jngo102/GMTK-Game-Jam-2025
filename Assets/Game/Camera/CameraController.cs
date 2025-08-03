using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
///     A camera that follows a target.
/// </summary>
[RequireComponent(typeof(Camera))]
[RequireComponent(typeof(Shaker))]
public class CameraController : MonoBehaviour
{
    [SerializeField] private float smoothing = 0.5f;
    [SerializeField] private Vector3 offset;
    [SerializeField] private float minHeight = 8;
    [SerializeField] private float maxHeight = 32;

    private List<Transform> targets = new();
    private new Camera camera;
    public Shaker shaker;
    private Vector3 velocity;

    private void Awake()
    {
        camera = GetComponent<Camera>();
        shaker = GetComponent<Shaker>();
    }

    private void LateUpdate()
    {
        FilterTargets();
        KeepTargetsInFrame();
    }

    /// <summary>
    ///     Add a target to the camera controller's list of targets to follow.
    /// </summary>
    /// <param name="target"></param>
    /// <param name="immediate">Whether to snap immediately to the new target.</param>
    public void AddTarget(Transform target, bool immediate = false)
    {
        targets.Add(target);
        if (immediate)
        {
            FollowPosition();
            FollowZoom();
        }
    }

    public void SetTargets(params Transform[] newTargets)
    {
        targets = newTargets.ToList();
    }

    /// <summary>
    ///     Remove a target from the camera controller's list of targets to follow.
    /// </summary>
    /// <param name="target"></param>
    public void RemoveTarget(Transform target)
    {
        targets.Remove(target);
    }

    /// <summary>
    ///     Reset the camera to the targets' positions.
    /// </summary>
    private void ResetPosition()
    {
        FollowPosition();
        FollowZoom();
    }

    /// <summary>
    ///     Keep all targets in the camera's view.
    /// </summary>
    private void KeepTargetsInFrame()
    {
        FollowPosition();
        FollowZoom();
    }

    /// <summary>
    ///     Follow targets, modifying the camera's position.
    /// </summary>
    private void FollowPosition(float? smooth = null)
    {
        if (targets.Count <= 0) return;

        var centerPoint = GetCenterPoint();
        var newPos = centerPoint + offset;
        transform.position = Vector3.Lerp(transform.position, newPos, smooth ?? smoothing);
        
    }

    private void FilterTargets()
    {
        for (var i = targets.Count - 1; i >= 0; i--)
        {
            var target = targets[i];
            if (!target)
            {
                RemoveTarget(target);
            }
        }
    }

    /// <summary>
    ///     Follow targets, modifying the camera's zoom.
    /// </summary>
    private void FollowZoom(float? smooth = null)
    {
        if (targets.Count <= 0) return;

        var newZoom = GetTargetZoom() / 2;
        camera.orthographicSize = Mathf.Lerp(camera.orthographicSize, newZoom, smooth ?? smoothing);
    }

    /// <summary>
    /// Get the maximum horizontal distance between all targets.
    /// </summary>
    /// <returns>The maximum horizontal distance between all targets.</returns>
    private float GetTargetZoom()
    {
        if (targets.Count <= 0)
        {
            return minHeight;
        }

        var maxTargetY = targets.Max(target => target.transform.position.y);
        var minTargetY = targets.Min(target => target.transform.position.y);
        var height = maxTargetY - minTargetY;
        return Mathf.Max(minHeight, Mathf.Min(height, maxHeight));
    }

    /// <summary>
    /// Get the center point among all targets.
    /// </summary>
    /// <returns>The center point among all targets.</returns>
    private Vector3 GetCenterPoint()
    {
        FilterTargets();
        if (targets.Count <= 0)
        {
            return Vector3.zero;
        }

        if (targets.Count == 1)
        {
            return targets[0].position;
        }

        var maxTargetX = targets.Max(target => target.transform.position.x);
        var maxTargetY = targets.Max(target => target.transform.position.y);
        var minTargetX = targets.Min(target => target.transform.position.x);
        var minTargetY = targets.Min(target => target.transform.position.y);
        return new Vector3(minTargetX + (maxTargetX - minTargetX) / 2, minTargetY + (maxTargetY - minTargetY) / 2, 0);
    }
}