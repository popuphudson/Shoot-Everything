using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExaustTriggerable : Triggerable
{
    [SerializeField] private Map1EEManager _eeManager;
    public override void Trigger() {
        if(_eeManager.GetEEState() == Map1EEState.START) {
            _eeManager.SetEEState(Map1EEState.EXHAUST);
        } else if(_eeManager.GetEEState() == Map1EEState.EXHAUST) {
            _eeManager.SetEEState(Map1EEState.START);
        }
    }
}
