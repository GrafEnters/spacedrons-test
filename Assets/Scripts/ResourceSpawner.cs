using UnityEngine;

public class ResourceSpawner : MonoBehaviour {
    [SerializeField]
    private float _spawnRadius = 10f, _verticalRadius = 3f;

    [SerializeField]
    private GameObject _resourcePrefab;

    private float _spawnInterval;
    private float _timer;
    private bool _inited;

    [SerializeField]
    private bool _is2d = false;

    public void SetData(float spawnInterval) {
        _spawnInterval = spawnInterval;
        _inited = true;
    }

    void Update() {
        if (!_inited) {
            return;
        }

        _timer += Time.deltaTime;
        if (_timer >= _spawnInterval) {
            _timer = 0f;
            SpawnResource();
        }
    }

    void SpawnResource() {
        Vector3 randomPos = transform.position + Random.insideUnitSphere * _spawnRadius;
        if (_is2d) {
            randomPos.y = 0f;
        } else {
            randomPos.y = Random.Range(0f, _verticalRadius);
        }

        Instantiate(_resourcePrefab, randomPos, Quaternion.identity);
    }
}