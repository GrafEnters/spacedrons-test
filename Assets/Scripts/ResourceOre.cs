using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class ResourceOre : MonoBehaviour {
    public bool IsTaken { get; set; }

    [SerializeField]
    private float _minCollectedScale = 0.5f;

    [SerializeField]
    private GameObject _resourceCubePrefab;

    private float _hpLeft = 1;

    private void Awake() {
        transform.up = Random.insideUnitSphere.normalized;
    }

    public GameObject DestroyItselfAndDropCube() {
        var resourceCube = Instantiate(_resourceCubePrefab, transform.position, Quaternion.identity);
        Destroy(gameObject);
        return resourceCube;
    }

    public void SetCollectedPercent(float percent) {
        if (_hpLeft > 1 - percent) {
            _hpLeft = 1 - percent;
        } else {
            return;
        }

        transform.localScale = Vector3.one * Mathf.Lerp(1f, _minCollectedScale, percent);
    }
}