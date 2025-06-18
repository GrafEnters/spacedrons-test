using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FactionBase : MonoBehaviour {
    [SerializeField]
    private TextMeshPro _counterText;

    [SerializeField]
    private DroneController _dronePrefab;

    private int _resourceCount;
    private int _dronesAmount;
    private FactionConfig _factionConfig;
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
                ReleaseDronToPool();
            }
        } else if (newValue > _activeDrones.Count) {
            for (int i = _activeDrones.Count; i < newValue; i++) {
                GetDronFromPoolOrCreate();
            }
        }
    }

    private void GetDronFromPoolOrCreate() {
        DroneController drone = _dronesPool.Count > 0 ? GetDronFromPool() : CreateNewDrone();
        _activeDrones.Enqueue(drone);
    }

    private DroneController GetDronFromPool() {
        DroneController drone = _dronesPool.Dequeue();
        drone.transform.position = transform.position;
        drone.gameObject.SetActive(true);
        return drone;
    }

    private DroneController CreateNewDrone() {
        DroneController drone = Instantiate(_dronePrefab, transform.position, Quaternion.identity);
        drone.SetData(_factionConfig, _simulationData, this);
        return drone;
    }

    private void ReleaseDronToPool() {
        var drone = _activeDrones.Dequeue();
        drone.ReleaseToPool();
        _dronesPool.Enqueue(drone);
    }

    public void AddResource() {
        _resourceCount++;
        _counterText.text = _resourceCount.ToString();
    }
}