using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerManager : MonoBehaviour
{
    [SerializeField] private GameObject[] _lights;
    private bool _mapPowered;

    public void PowerMap() {
        _mapPowered = true;
        foreach(GameObject light in _lights) {
            light.SetActive(true);
        }
    }

    public bool IsMapPowered() {
        return _mapPowered;
    }


}
