using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map1ReturnToMapTriggerable : Triggerable
{
    [SerializeField] private Map1EEManager _eeManager;
    [SerializeField] private ZombieSpawner _spawner;
    public override void Trigger() {
        _eeManager.SetEEState(Map1EEState.RANGE_EXTEND);
        _spawner.Resume();
    }
}
