using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockDownRequirementTriggerable : Triggerable
{
    [SerializeField] private LockDownHandler _lockDown;
    public override void Trigger() {
        _lockDown.DecrementLockDownNum();
    }
}
