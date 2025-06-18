using UnityEngine;
using System.Collections;

public class DroneController : MonoBehaviour {
    public enum State {
        Idle,
        ToResource,
        Collecting,
        ToBase,
        Unloading
    }

    private State _currentState = State.Idle;

    private float _speed;

    private Transform _targetResource;
    private FactionBase _base;
    private Vector3 _basePosition;

    private float _collectTimer;

    [SerializeField]
    private Renderer _renderer;

    [SerializeField]
    private float _collectionDuration = 2f;

    [SerializeField]
    private float _avoidanceRadius = 1f;

    [SerializeField]
    private float _avoidanceStrength = 2f;

    [SerializeField]
    private float _collectDistance = 0.1f;
    [SerializeField]
    private GameObject _pickUpEffect, _unloadEffect;

    private GameObject _fxObj;
    
    void Update() {
        AvoidCollision();

        switch (_currentState) {
            case State.Idle:
                FindResource();
                break;
            case State.ToResource:
                if (_targetResource == null) {
                    _currentState = State.Idle;
                    return;
                }

                MoveTo(_targetResource.position);
                if (Vector3.Distance(transform.position, _targetResource.position) <= _collectDistance) {
                    _currentState = State.Collecting;
                    _collectTimer = 0f;
                }

                break;
            case State.Collecting:
                Collecting();

                break;
            case State.ToBase:
                MoveTo(_basePosition);
                if (Vector3.Distance(transform.position, _basePosition) < 0.1f) {
                    _currentState = State.Unloading;
                    StartCoroutine(UnloadEffect());
                }

                break;
            case State.Unloading:
                // В процессе выгрузки — ожидание в корутине
                break;
        }
    }

    private void Collecting() {
        _collectTimer += Time.deltaTime;

        if (_fxObj == null) {
            _fxObj = Instantiate(_pickUpEffect, transform.position, Quaternion.identity);
            _fxObj.transform.forward =  _targetResource.position - transform.position;
        }
      
        if (_collectTimer >= _collectionDuration) {
            if (_targetResource != null) {
                Destroy(_targetResource.gameObject);
                _targetResource = null;
                if (_fxObj != null) {
                    Destroy(_fxObj);
                    _fxObj = null;
                }
            }

            _currentState = State.ToBase;
        }
    }

    void MoveTo(Vector3 target) {
        Vector3 dir = (target - transform.position).normalized;

        float rayDistance = 1f;
        RaycastHit hit;

        // Проверяем впереди препятствие
        if (Physics.Raycast(transform.position, dir, out hit, rayDistance)) {
            if (hit.collider.gameObject != _targetResource?.gameObject) {
                if (hit.collider.gameObject.CompareTag("Resource") || hit.collider.gameObject.CompareTag("Obstacle")) {
                    // Сдвигаем направление вправо, чтобы обойти препятствие
                    Vector3 right = Vector3.Cross(dir, Vector3.up);
                    dir = (dir + right).normalized;
                }
            }
        }

        transform.position += dir * _speed * Time.deltaTime;
    }

    void FindResource() {
        var resources = FindObjectsByType<Resource>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        float minDist = float.MaxValue;
        Resource nearest = null;

        foreach (var res in resources) {
            if (res.IsTaken) continue;
            float dist = Vector3.Distance(transform.position, res.transform.position);
            if (dist < minDist) {
                minDist = dist;
                nearest = res;
            }
        }

        if (nearest != null) {
            _targetResource = nearest.transform;
            nearest.IsTaken = true;
            _currentState = State.ToResource;
        }
    }

    void AvoidCollision() {
        Collider[] hits = Physics.OverlapSphere(transform.position, _avoidanceRadius);
        Vector3 avoidDir = Vector3.zero;
        int count = 0;

        foreach (var hit in hits) {
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
            avoidDir /= count;
            transform.position += avoidDir.normalized * _avoidanceStrength * Time.deltaTime;
        }
    }

    IEnumerator UnloadEffect() {
        if (_unloadEffect != null) {
           var obj = Instantiate(_unloadEffect, transform.position, Quaternion.identity);
           var particle = obj.GetComponent<ParticleSystem>().main;
           particle.startColor = _renderer.material.color * 0.5f;
           obj.transform.forward = Vector3.up;
        }
        // Здесь можно добавить эффект частиц, вспышку, масштабирование и т.д.
        // Например, масштабируем дрона на 1.2 и обратно в 0.5 сек
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
        _base.AddResource();
        _currentState = State.Idle;
    }

    public void SetData(FactionConfig factionConfig, float speed, FactionBase baseObj) {
        _speed = speed;
        _base = baseObj;
        _renderer.material.color = factionConfig.Color;
        _basePosition = baseObj.transform.position;
    }
}