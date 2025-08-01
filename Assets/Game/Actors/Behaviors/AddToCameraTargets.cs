using UnityEngine;

public class AddToCameraTargets : MonoBehaviour
{
    private void Start()
    {
        var mainCam = Camera.main;
        if (mainCam && mainCam.TryGetComponent<CameraController>(out var cameraController))
        {
            cameraController.AddTarget(transform, true);
        }
    }
}
