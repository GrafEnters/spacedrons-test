using System.Collections.Generic;
using UnityEngine;

public class SimulationTimeController {
    private SimulationData _simulationData;

    public List<float> SimulationSpeeds { get; } = new() { 0.25f, 0.5f, 1f, 2f, 4f, 8f };

    public void SetData(SimulationData simulationData) {
        _simulationData = simulationData;
        _simulationData.OnSimulationSpeedIndexChanged += OnSimulationSpeedIndexChanged;
    }

    private void OnSimulationSpeedIndexChanged(int newValue) {
        Time.timeScale = SimulationSpeeds[newValue];
    }
}