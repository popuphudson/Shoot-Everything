using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    private Transform _playerCamera;
    void Start()
    {
        _playerCamera = Camera.main.transform;
    }

    void Update()
    {
        transform.LookAt(_playerCamera);
    }
}
