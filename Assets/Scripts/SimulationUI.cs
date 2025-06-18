using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class SimulationUI : MonoBehaviour {
    [SerializeField]
    private Slider _dronesBlue;

    [SerializeField]
    private Slider _dronesRed;

    [SerializeField]
    private Slider _dronesSpeed;

    [SerializeField]
    private Slider _spawnInterval;

    [SerializeField]
    private Slider _simulationSpeed;

    [SerializeField]
    private DroneController _dronePrefab;

    [SerializeField]
    private ResourceSpawner _spawner;

    [SerializeField]
    private FactionBase _baseBlue, _baseRed;

    [SerializeField]
    private FactionConfig _factionConfigBlue, _factionConfigRed;

    [SerializeField]
    private Toggle _pathToggle;

    [SerializeField]
    private TextMeshProUGUI _simulationSpeedLabel;
    
    private List<float> _simulationSpeeds = new List<float> { 0.25f, 0.5f, 1f, 2f, 4f, 8f };

    private void Start() {
        _simulationSpeed.maxValue = _simulationSpeeds.Count - 1;
        _simulationSpeed.SetValueWithoutNotify(_simulationSpeeds.IndexOf(1));
    }

    public void StartSimulation() {
        _baseBlue.SetData(_factionConfigBlue);
        _baseRed.SetData(_factionConfigRed);
        SpawnDrones((int)_dronesBlue.value, _baseBlue, _factionConfigBlue);
        SpawnDrones((int)_dronesRed.value, _baseRed, _factionConfigRed);
        _spawner.SetData(_spawnInterval.value);
        DroneController.SetPathVisibility(_pathToggle.isOn);
    }

    void SpawnDrones(int count, FactionBase baseT, FactionConfig factionConfig) {
        for (int i = 0; i < count; i++) {
            Vector3 pos = baseT.transform.position + Random.insideUnitSphere * 2f;
            pos.y = 0f;
            DroneController drone = Instantiate(_dronePrefab, pos, Quaternion.identity);
            drone.SetData(factionConfig, _dronesSpeed.value, baseT);
        }
    }

    public void OnPathsToggleChanged(bool isOn) {
        DroneController.SetPathVisibility(isOn);
    }
    
    public void OnSimulationSpeedChanged(float value) {
        Time.timeScale = _simulationSpeeds[(int)value];
        _simulationSpeedLabel.text = $"Simulation speed: {Time.timeScale}x";
    }
}