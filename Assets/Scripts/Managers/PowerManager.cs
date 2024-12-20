using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PowerManager : MonoBehaviour
{
    [SerializeField] private Material _lightedMaterial;
    [SerializeField] private GameObject[] _lights;
    [SerializeField] private MeshRenderer[] _lightObjects;
    [SerializeField] private UnityEvent _powerOn;
    private bool _mapPowered;

    public void PowerMap() {
        _mapPowered = true;
        foreach(GameObject light in _lights) {
            light.SetActive(true);
        }
        foreach(MeshRenderer lightObjects in _lightObjects) {
            lightObjects.material = _lightedMaterial;
        }
        _powerOn.Invoke();
    }

    public bool IsMapPowered() {
        return _mapPowered;
    }


}
