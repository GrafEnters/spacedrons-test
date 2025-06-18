using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FactionBase : MonoBehaviour {
    [SerializeField]
    private TextMeshPro _counterText;

    [SerializeField]
    private DroneController _dronePrefab;

    private FactionConfig _factionConfig;
    private int _resourceCount;
    private int _dronesAmount;
    private Queue<DroneController> _activeDrones = new Queue<DroneController>();
    private Queue<DroneController> _dronesPool = new Queue<DroneController>();
    private SimulationData _simulationData;

    public void SetData(FactionConfig factionConfig, SimulationData simulationData) {
        _simulationData = simulationData;
        _factionConfig = factionConfig;
        _counterText.text = "0";
        _counterText.color = factionConfig.Color;
        _resourceCount = 0;
        if (_factionConfig.FactionType == FactionType.Blue) {
            _simulationData.OnBlueDronesCountChanged += ChangeDronsAmount;
            ChangeDronsAmount(_simulationData.BlueDronesCount);
        } else {
            _simulationData.OnRedDronesCountChanged += ChangeDronsAmount;
            ChangeDronsAmount(_simulationData.RedDronesCount);
        }
    }

    public void ChangeDronsAmount(int newValue) {
        if (newValue < _activeDrones.Count) {
            for (int i = _activeDrones.Count - 1; i >= newValue; i--) {
                var drone = _activeDrones.Dequeue();
                drone.ReleaseToPool();
                _dronesPool.Enqueue(drone);
            }
        } else if (newValue > _activeDrones.Count) {
            for (int i = _activeDrones.Count; i < newValue; i++) {
                DroneController drone;
                if (_dronesPool.Count > 0) {
                    drone = _dronesPool.Dequeue();
                    drone.transform.position = transform.position;
                    drone.gameObject.SetActive(true);
                } else {
                    drone = Instantiate(_dronePrefab, transform.position, Quaternion.identity);
                    drone.SetData(_factionConfig, _simulationData, this);
                }

                _activeDrones.Enqueue(drone);
            }
        }
    }

    public void AddResource() {
        _resourceCount++;
        _counterText.text = _resourceCount.ToString();
    }
}