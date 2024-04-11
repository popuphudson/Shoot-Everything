using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    private PowerUpManager _manager;
    private PowerupType _powerupType;

    public void SetPowerUpManager(PowerUpManager manager) {
        _manager = manager;
    }

    public void SetPowerUpType(PowerupType powerupType) {
        _powerupType = powerupType;
    }

    private void OnTriggerEnter(Collider other) {
        if(other.CompareTag("Player")) {
            _manager.ActivatePowerUp(_powerupType);
            Destroy(gameObject);
        }
    }
}
