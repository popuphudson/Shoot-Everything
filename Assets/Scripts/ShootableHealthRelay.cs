using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootableHealthRelay : MonoBehaviour
{
    [SerializeField] private Shootable _shootable;
    public void ShowHealth() {
        _shootable.ShowHealth();
    }
}
