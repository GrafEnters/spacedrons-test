using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SimulationUI : MonoBehaviour {
    [SerializeField]
    private Slider _simulationSpeed;

    [SerializeField]
    private TextMeshProUGUI _simulationSpeedLabel;

    private SimulationData _simulationData;

    public void SetData(SimulationData simulationData, List<float> simulationSpeeds) {
        _simulationData = simulationData;
        InitTimescaleSlider(simulationSpeeds);
    }

    private void InitTimescaleSlider(List<float> simulationSpeeds) {
        _simulationSpeed.maxValue = simulationSpeeds.Count - 1;
        _simulationSpeed.SetValueWithoutNotify(simulationSpeeds.IndexOf(1));
    }

    public void StartSimulation() {
        _simulationData.OnStartSimulation?.Invoke();
    }

    public void OnBlueDronesAmountChanged(float value) {
        _simulationData.BlueDronesCount = (int)value;
    }

    public void OnRedDronesAmountChanged(float value) {
        _simulationData.RedDronesCount = (int)value;
    }

    public void OnDronesSpeedChanged(float value) {
        _simulationData.DroneSpeed = value;
    }

    public void OnResourceSpawnIntervalChanged(float value) {
        _simulationData.SpawnResourceInterval = value;
    }

    public void OnSimulationSpeedChanged(float value) {
        _simulationData.SimulationSpeedIndex = (int)value;
        _simulationSpeedLabel.text = $"Simulation speed: {Time.timeScale}x";
    }

    public void OnPathsToggleChanged(bool isOn) {
        _simulationData.IsShowPaths = isOn;
    }
}