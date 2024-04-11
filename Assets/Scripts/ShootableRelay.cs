using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootableRelay : MonoBehaviour
{
    [SerializeField] private Shootable _shootable;
    [SerializeField] private float _damageMul = 1;
    [SerializeField] private float _pointMul = 1;

    public void TakeDamage(float damage, PlayerPoints playerPoints, bool melee, PowerUpManager powerUpManager) {
        if(melee) {
            _shootable.TakeDamage(damage, playerPoints, 1.23f, powerUpManager);
        } else {
            _shootable.TakeDamage(damage*_damageMul, playerPoints, _pointMul, powerUpManager);
        }
    }
}
