using UnityEngine;

public class ResourceOre : MonoBehaviour {
    public bool IsTaken { get; set; }

    [SerializeField]
    private float _minCollectedScale = 0.5f;

    [SerializeField]
    private GameObject _resourceCubePrefab;

    public GameObject DestroyItselfAndDropCube() {
        var resourceCube = Instantiate(_resourceCubePrefab, transform.position, Quaternion.identity);
        Destroy(gameObject);
        return resourceCube;
    }

    public void SetCollectedPercent(float percent) {
        transform.localScale = Vector3.one * Mathf.Lerp(1f, _minCollectedScale, percent);
    }
}