using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Map1EEState {
    START,
    EXHAUST,
    KEYCARD,
    LOCKDOWN,
    RANGE_EXTEND,
    BOSS_FIGHT
}

public class Map1EEManager : MonoBehaviour
{
    [SerializeField] private ExaustShootable[] _exausts;
    [SerializeField] private CacheShootable _cache;
    [SerializeField] private Vector3[] _cachePositions;
    [SerializeField] private Vector3[] _cacheRotations;
    [SerializeField] private Map1EEState _state = Map1EEState.START;
    private bool _allExaustsActive;
    
    public void SetEEState(Map1EEState __state) {
        _state = __state;
    }

    public Map1EEState GetEEState() {
        return _state;
    }

    private void Start() {
        UpdateEEStep();
    }

    public bool GetExaustStepComplete() {
        return _allExaustsActive;
    }

    public void ProceedFromExaustStep() {
        _cache.Unlock();
        _state = Map1EEState.KEYCARD;
    }

    public void TryStartExaustStep() {
        if(_state == Map1EEState.START) SetEEState(Map1EEState.EXHAUST);
    }

    public void TryStopExaustStep() {
        if(_state == Map1EEState.EXHAUST) SetEEState(Map1EEState.START);
    }

    public void StartLockDownStep() {
        SetEEState(Map1EEState.LOCKDOWN);
    }

    public void StartRangeExtendStep() {
        SetEEState(Map1EEState.RANGE_EXTEND);
    }

    public void UpdateEEStep() {
        if(_state == Map1EEState.EXHAUST) {
            bool allActive = true;
            foreach(ExaustShootable exaust in _exausts) {
                if(!exaust.GetActive()) allActive = false;
            }
            _allExaustsActive = allActive;
        }
    }
}
