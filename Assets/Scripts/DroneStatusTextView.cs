using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DroneStatusTextView : MonoBehaviour {
    [SerializeField]
    private TextMeshPro _statusText;

    private Dictionary<DronState, string> _statusMessages = new Dictionary<DronState, string> {
        { DronState.Idle, "Ищет\nастероид" },
        { DronState.ToResource, "Летит к\nастероиду" },
        { DronState.Collecting, "Добывает\nастероид" },
        { DronState.ToBase, "Возвращается\nна базу" },
        { DronState.Unloading, "Складывает\nресурсы" }
    };

    public void SetStatusText(DronState state) {
        if (!gameObject.activeSelf) {
            return;
        }

        _statusText.text = _statusMessages[state];
    }
}