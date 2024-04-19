using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerManager : MonoBehaviour
{
    private bool _mapPowered;

    public void PowerMap() {
        _mapPowered = true;
    }

    public bool IsMapPowered() {
        return _mapPowered;
    }


}
