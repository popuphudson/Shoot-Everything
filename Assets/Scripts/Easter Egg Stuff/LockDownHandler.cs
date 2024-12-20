using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LockDownHandler : MonoBehaviour
{
    [SerializeField] private int _requiredOfType;
    [SerializeField] private ZombieSpawner _spawner;
    [SerializeField] private UnityEvent _onLockDownEnd;
    private int _leftOfRequiredType;
    private bool _lockDowning = false;
    public void StartLockDown() {
        _spawner.KillAll();
        _spawner.SetSpawnSpeed(0.5f);
        _leftOfRequiredType = _requiredOfType;
        _lockDowning = true;
    }

    public void DecrementLockDownNum() {
        if(!_lockDowning) return;
        _leftOfRequiredType--;
        if(_leftOfRequiredType < 0) {
            EndLockDown();
        }
    }

    private void EndLockDown() {
        _lockDowning = false;
        _onLockDownEnd.Invoke();
        _spawner.SetSpawnSpeed(-1);
    }
}
