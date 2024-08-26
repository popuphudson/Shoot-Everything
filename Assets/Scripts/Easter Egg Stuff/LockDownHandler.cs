using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockDownHandler : MonoBehaviour
{
    [SerializeField] private int _requiredOfType;
    [SerializeField] private Triggerable[] _lockDownEndTriggerable;
    private int _leftOfRequiredType;
    private bool _lockDowning = false;
    public void StartLockDown() {
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
        foreach(Triggerable triggerable in _lockDownEndTriggerable) {
            triggerable.Trigger();
        }
    }
}
