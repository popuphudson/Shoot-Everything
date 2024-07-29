using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootableRelay : MonoBehaviour
{
    [SerializeField] private Shootable _shootable;
    [SerializeField] private float _damageMul = 1;
    [SerializeField] private float _pointMul = 1;

    public void TakeDamage(float __damage, PlayerPoints __playerPoints, bool __melee, PowerUpManager __powerUpManager) {
        if(__melee) {
            _shootable.TakeDamage(__damage, __playerPoints, 13/6, __powerUpManager);
        } else {
            _shootable.TakeDamage(__damage*_damageMul, __playerPoints, _pointMul, __powerUpManager);
        }
    }

    public float GetShootableHealth() {
        return _shootable.GetHealth();
    }

    public float GetShootableMaxHealth() {
        return _shootable.GetMaxHealth();
    }
}
