using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockDownTriggerable : Triggerable
{
    [SerializeField] private Map1EEManager _eeManager;
    public override void Trigger() {
        _eeManager.SetEEState(Map1EEState.LOCKDOWN);
    }
}
