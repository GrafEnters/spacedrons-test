using UnityEngine;

public class TextCameraAutoAligner : MonoBehaviour {
    private Transform _mainCameraTransform;

    private void Awake() {
        _mainCameraTransform = Camera.main.transform;
    }

    public void LateUpdate() {
        RotateTextToCamera();
    }

    private void RotateTextToCamera() {
        transform.forward = _mainCameraTransform.forward;
    }
}