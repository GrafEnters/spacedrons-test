using UnityEngine;
using TMPro;

public class FactionBase : MonoBehaviour {
    [SerializeField]
    private TextMeshPro _counterText;

    private int _resourceCount;

    public void SetData(FactionConfig factionConfig) {
        _counterText.text = "0";
        _counterText.color = factionConfig.Color;
        _resourceCount = 0;
    }

    public void AddResource() {
        _resourceCount++;
        _counterText.text = _resourceCount.ToString();
    }
}