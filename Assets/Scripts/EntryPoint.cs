using System;
using UnityEngine;

public class EntryPoint : MonoBehaviour {
    [SerializeField]
    private SimulationUI _simulationUI;

    [SerializeField]
    private FactionBase _baseBlue, _baseRed;

    [SerializeField]
    private FactionConfig _factionConfigBlue, _factionConfigRed;

    [SerializeField]
    private ResourceSpawner _resourceSpawner;

    private readonly SimulationData _simulationData = new();
    private readonly SimulationTimeController _simulationTimeController = new();

    private void Awake() {
        _simulationTimeController.SetData(_simulationData);
        _simulationUI.SetData(_simulationData, _simulationTimeController.SimulationSpeeds);
        _baseBlue.SetData(_factionConfigBlue, _simulationData);
        _baseRed.SetData(_factionConfigRed, _simulationData);
        _resourceSpawner.SetData(_simulationData);
    }
}