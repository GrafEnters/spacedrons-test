using System;
using UnityEngine;

public class SimulationData {
    private int _blueDronesCount = 10;
    private int _redDronesCount = 10;
    private float _droneSpeed = 5;
    private bool _isShowPaths = false;
    private int _simulationSpeedIndex = 3;
    private float _spawnResourceInterval = 0.5f;

    public int BlueDronesCount {
        get => _blueDronesCount;
        set {
            int clamped = Math.Max(0, value);
            if (_blueDronesCount != clamped) {
                _blueDronesCount = clamped;
                OnBlueDronesCountChanged?.Invoke(_blueDronesCount);
            }
        }
    }

    public int RedDronesCount {
        get => _redDronesCount;
        set {
            int clamped = Math.Max(0, value);
            if (_redDronesCount != clamped) {
                _redDronesCount = clamped;
                OnRedDronesCountChanged?.Invoke(_redDronesCount);
            }
        }
    }

    public float DroneSpeed {
        get => _droneSpeed;
        set {
            if (!Mathf.Approximately(_droneSpeed, value)) {
                _droneSpeed = value;
                OnDroneSpeedChanged?.Invoke(_droneSpeed);
            }
        }
    }

    public bool IsShowPaths {
        get => _isShowPaths;
        set {
            if (_isShowPaths != value) {
                _isShowPaths = value;
                OnShowPathsChanged?.Invoke(_isShowPaths);
            }
        }
    }

    public int SimulationSpeedIndex {
        get => _simulationSpeedIndex;
        set {
            if (_simulationSpeedIndex != value) {
                _simulationSpeedIndex = value;
                OnSimulationSpeedIndexChanged?.Invoke(_simulationSpeedIndex);
            }
        }
    }

    public float SpawnResourceInterval {
        get => _spawnResourceInterval;
        set {
            if (!Mathf.Approximately(_spawnResourceInterval, value)) {
                _spawnResourceInterval = value;
                OnSpawnResourceIntervalChanged?.Invoke(_spawnResourceInterval);
            }
        }
    }

    public Action<int> OnBlueDronesCountChanged;
    public Action<int> OnRedDronesCountChanged;
    public Action<float> OnDroneSpeedChanged;
    public Action<bool> OnShowPathsChanged;
    public Action<int> OnSimulationSpeedIndexChanged;
    public Action<float> OnSpawnResourceIntervalChanged;

    public Action OnStartSimulation;
}