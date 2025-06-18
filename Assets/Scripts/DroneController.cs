using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DroneController : MonoBehaviour {
    [SerializeField]
    private float _collectionDuration = 2f;

    [SerializeField]
    private float _avoidanceRadius = 1f;

    [SerializeField]
    private float _avoidanceStrength = 2f;

    [SerializeField]
    private float _collectDistance = 0.1f, _unloadDistance = 0.5f;

    [SerializeField]
    private float _evadeDistance = 1f;

    [SerializeField]
    private Renderer _renderer;

    [SerializeField]
    private GameObject _pickUpEffect, _unloadEffect;

    [SerializeField]
    private LineRenderer _lineRenderer;

    [SerializeField]
    private DroneStatusTextView _statusTextView;

    private DronState _currentDronState = DronState.Idle;

    private ResourceOre _targetResource;
    private FactionBase _base;
    private Vector3 _basePosition;

    private float _collectTimer;
    private GameObject _fxObj;
    private List<Vector3> _pathPoints = new();
    private GameObject _carryingCube;
    private SimulationData _simulationData;

    public void SetData(FactionConfig factionConfig, SimulationData data, FactionBase baseObj) {
        _base = baseObj;
        _simulationData = data;
        _renderer.material.color = factionConfig.Color;
        _basePosition = baseObj.transform.position;
    }

    private void Update() {
        SwitchFsmState();
        UpdatePathLine();
        _statusTextView.SetStatusText(_currentDronState);
    }

    private void SwitchFsmState() {
        switch (_currentDronState) {
            case DronState.Idle:
                FindResource();
                break;
            case DronState.ToResource:
                if (_targetResource == null) {
                    _currentDronState = DronState.Idle;
                    break;
                }

                AvoidCollisionWithDrones();
                _pathPoints = RebuildPath(transform.position, _targetResource.transform.position, _collectDistance * 0.9f);
                MoveTo(_pathPoints[1]);
                if (Vector3.Distance(transform.position, _targetResource.transform.position) <= _collectDistance) {
                    _currentDronState = DronState.Collecting;
                    SpawnCollectionFx();
                    _collectTimer = 0f;
                }

                break;
            case DronState.Collecting:
                Collecting();

                break;
            case DronState.ToBase:
                // Если дрон далеко от базы, то избегаем столкновений с другими дронами (внутри базы они сталкиваются)
                if (Vector3.Distance(transform.position, _basePosition) > _unloadDistance * 4f) {
                    AvoidCollisionWithDrones();
                }

                _pathPoints = RebuildPath(transform.position, _basePosition, _unloadDistance * 0.5f);
                MoveTo(_pathPoints[1]);
                if (Vector3.Distance(transform.position, _basePosition) <= _unloadDistance) {
                    _currentDronState = DronState.Unloading;
                    StartCoroutine(UnloadEffect());
                }

                break;
            case DronState.Unloading:
                // В процессе выгрузки — ожидание в корутине
                break;
        }
    }

    private void UpdatePathLine() {
        if (!_simulationData.IsShowPaths || _currentDronState == DronState.Idle) {
            _lineRenderer.enabled = false;
            return;
        }

        _lineRenderer.enabled = true;
        _lineRenderer.positionCount = _pathPoints.Count;
        for (int i = 0; i < _pathPoints.Count; i++) {
            _lineRenderer.SetPosition(i, _pathPoints[i]);
        }
    }

    private List<Vector3> RebuildPath(Vector3 start, Vector3 end, float stopDistance = 0.5f, int maxSteps = 10) {
        return Pathfinder.BuildPath(start, end, _evadeDistance, stopDistance, maxSteps);
    }

    private void Collecting() {
        _collectTimer += Time.deltaTime;

        if (_targetResource != null) {
            float percent = Mathf.Clamp01(_collectTimer / _collectionDuration);
            _targetResource.SetCollectedPercent(percent);
        }

        if (!(_collectTimer >= _collectionDuration)) {
            return;
        }

        if (_targetResource != null) {
            GameObject spawnedCube = _targetResource.DestroyItselfAndDropCube();
            CollectCube(spawnedCube);

            _targetResource = null;
            if (_fxObj != null) {
                Destroy(_fxObj);
                _fxObj = null;
            }
        }

        _currentDronState = DronState.ToBase;
    }

    private void SpawnCollectionFx() {
        _fxObj = Instantiate(_pickUpEffect, transform.position, Quaternion.identity);
        _fxObj.GetComponent<ParticleSystemRenderer>().trailMaterial = _renderer.material;
        _fxObj.transform.forward = _targetResource.transform.position - transform.position;
    }

    private void CollectCube(GameObject cube) {
        Transform cubeTransform = cube.transform;
        cubeTransform.SetParent(transform);
        cubeTransform.localPosition = Vector3.up * 0.1f;
        _carryingCube = cube;
    }

    private void MoveTo(Vector3 target) {
        Vector3 dir = (target - transform.position).normalized;

        float rayDistance = 1f;

        // Проверяем впереди препятствие
        if (Physics.Raycast(transform.position, dir, out RaycastHit hit, rayDistance)) {
            if (hit.collider.gameObject != _targetResource?.gameObject) {
                if (hit.collider.gameObject.CompareTag("Resource") || hit.collider.gameObject.CompareTag("Obstacle")) {
                    // Сдвигаем направление вправо, чтобы обойти препятствие
                    Vector3 right = Vector3.Cross(dir, Vector3.up);
                    dir = (dir + right).normalized;
                }
            }
        }

        transform.position += dir * _simulationData.DroneSpeed * Time.deltaTime;
    }

    private void FindResource() {
        var resources = FindObjectsByType<ResourceOre>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        float minDist = float.MaxValue;
        ResourceOre nearest = null;

        foreach (var res in resources) {
            if (res.IsTaken) continue;
            float dist = Vector3.Distance(transform.position, res.transform.position);
            if (dist < minDist) {
                minDist = dist;
                nearest = res;
            }
        }

        if (nearest != null) {
            _targetResource = nearest;
            nearest.IsTaken = true;
            _currentDronState = DronState.ToResource;
        }
    }

    private void AvoidCollisionWithDrones() {
        Collider[] hits = Physics.OverlapSphere(transform.position, _avoidanceRadius);
        Vector3 avoidDir = Vector3.zero;
        int count = 0;

        foreach (Collider hit in hits) {
            if (hit.gameObject == gameObject || hit.gameObject == _targetResource?.gameObject) {
                continue;
            }

            if (hit.attachedRigidbody == null) {
                continue;
            }

            if (hit.attachedRigidbody.TryGetComponent<DroneController>(out _)) {
                Vector3 diff = transform.position - hit.transform.position;
                if (diff.magnitude > 0) {
                    avoidDir += diff.normalized / diff.magnitude;
                    count++;
                }
            }
        }

        if (count > 0) {
            transform.position += avoidDir.normalized * _avoidanceStrength * Time.deltaTime;
        }
    }

    private IEnumerator UnloadEffect() {
        if (_unloadEffect != null) {
            var obj = Instantiate(_unloadEffect, transform.position, Quaternion.identity);
            var particle = obj.GetComponent<ParticleSystem>().main;
            particle.startColor = _renderer.material.color * 0.5f;
            obj.transform.forward = Vector3.up;
        }

        yield return StartCoroutine(ShowUnloadAnimation());

        Destroy(_carryingCube);
        _carryingCube = null;
        _base.AddResource();
        _currentDronState = DronState.Idle;
    }

    private IEnumerator ShowUnloadAnimation() {
        Vector3 originalScale = transform.localScale;
        Vector3 targetScale = originalScale * 1.2f;
        float elapsed = 0f;
        float duration = 0.5f;
        while (elapsed < duration) {
            transform.localScale = Vector3.Lerp(originalScale, targetScale, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        elapsed = 0f;
        while (elapsed < duration) {
            transform.localScale = Vector3.Lerp(targetScale, originalScale, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
    }

    public void ChangeFollowState(bool isFollowing) {
        _statusTextView.gameObject.SetActive(isFollowing);
    }

    public void ReleaseToPool() {
        gameObject.SetActive(false);
        if (_carryingCube) {
            Destroy(_carryingCube);
            _carryingCube = null;
        }

        if (_targetResource) {
            _targetResource.IsTaken = false;
        }

        transform.position = _basePosition;

        _currentDronState = DronState.Idle;
    }
}

[Serializable]
public enum DronState {
    Idle,
    ToResource,
    Collecting,
    ToBase,
    Unloading
}