using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Must;

public class CoilGunStunner : MonoBehaviour
{
    [SerializeField] private LayerMask _enemySplashLayer;
    [SerializeField] private LayerMask _playerLayer;
    [SerializeField] private float _stayTime = 5f;
    [SerializeField] private float _timeToKill = 1f;
    private PlayerPoints _playerPoints;
    private PowerUpManager _powerUpManager;

    private void Start() {
        Destroy(gameObject, _stayTime);
    }

    public void SetData(PlayerPoints __playerPoints, PowerUpManager __powerUpManager) {
        _playerPoints = __playerPoints;
        _powerUpManager = __powerUpManager;
    }

    void Update()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 2f, _enemySplashLayer);
        foreach(Collider col in hitColliders) {
            ShootableRelay shot = col.transform.GetComponent<ShootableRelay>();
            if(shot) {
                shot.TakeDamage(shot.GetShootableMaxHealth()*(Time.deltaTime/_timeToKill), _playerPoints, false, _powerUpManager);
            }
        }
        hitColliders = Physics.OverlapSphere(transform.position, 2f, _playerLayer);
        foreach(Collider col in hitColliders) {
            PlayerHealth playerHealth = col.transform.GetComponent<PlayerHealth>();
            if(playerHealth) {
                playerHealth.NonLeathalDamage(10);
            }
        }
    }
}
