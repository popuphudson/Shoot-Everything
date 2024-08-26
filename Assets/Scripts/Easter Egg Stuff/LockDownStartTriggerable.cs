using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockDownStartTriggerable : Triggerable
{
    [SerializeField] private LockDownHandler _lockDown;
    [SerializeField] private ZombieSpawner _spawner;
    public override void Trigger() {
        _spawner.KillAll();
        _lockDown.StartLockDown();
    }
}
