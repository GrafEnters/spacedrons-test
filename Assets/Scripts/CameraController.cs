using System;
using UnityEngine;
using Unity.Cinemachine;

public class CameraController : MonoBehaviour {
    [SerializeField]
    private CinemachineCamera _followCam, _staticCam;

    [SerializeField]
    private Transform _defaultTarget;

    [SerializeField]
    private LayerMask _droneLayer;

    private DroneController _followingDrone;

    private void Start() {
        ChangeCam(false);
    }

    private void Update() {
        if (Input.GetMouseButtonDown(0)) {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 100f, _droneLayer)) {
                ChangeCam(true);
                _followingDrone = hit.collider.attachedRigidbody.GetComponent<DroneController>();
                _followingDrone.ChangeFollowState(true);
                _followCam.LookAt = hit.transform;
                _followCam.Follow = hit.transform;
            }
        }

        if (Input.GetMouseButtonDown(1)) {
            ChangeCam(false);
            _followingDrone.ChangeFollowState(false);
            _followingDrone = null;
            _followCam.Follow = _defaultTarget;
            _followCam.LookAt = _defaultTarget;
        }
    }

    private void ChangeCam(bool isFollowing) {
        _followCam.enabled = isFollowing;
        _staticCam.enabled = !isFollowing;
    }
}