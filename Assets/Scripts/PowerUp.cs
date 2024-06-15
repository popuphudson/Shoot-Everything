using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PowerUp : MonoBehaviour
{
    [SerializeField] private TextMeshPro _powerUpText;
    private PowerUpManager _manager;
    private PowerupType _powerupType;

    private void Start() {
        StartCoroutine(Dissapear());
    }

    private IEnumerator Dissapear() {
        yield return new WaitForSeconds(10);
        for(int i = 0; i < 20; i++) {
            _powerUpText.alpha = 1;
            yield return new WaitForSeconds(0.1f);
            _powerUpText.alpha = 0;
            yield return new WaitForSeconds(0.1f);
        }
        for(int i = 0; i < 20; i++) {
            _powerUpText.alpha = 1;
            yield return new WaitForSeconds(0.05f);
            _powerUpText.alpha = 0;
            yield return new WaitForSeconds(0.05f);
        }
        Destroy(gameObject);
    }

    public void SetPowerUpManager(PowerUpManager __manager) {
        _manager = __manager;
    }

    public void SetPowerUpType(PowerupType __powerupType) {
        _powerupType = __powerupType;
    }

    private void OnTriggerEnter(Collider __other) {
        if(__other.CompareTag("Player")) {
            _manager.ActivatePowerUp(_powerupType);
            Destroy(gameObject);
        }
    }
}
