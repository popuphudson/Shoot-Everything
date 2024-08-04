using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    [SerializeField] private Sound _explodeSound;
    [SerializeField] private LayerMask _enemySplashLayer;
    [SerializeField] private LayerMask _playerLayer;
    [SerializeField] private float _damage = 1f;
    [SerializeField] private float _radius = 2f;
    private PlayerPoints _playerPoints;
    private PowerUpManager _powerUpManager;
    private AudioManager _audioManager;

    private void Start() {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, _radius, _enemySplashLayer);
        foreach(Collider col in hitColliders) {
            ShootableRelay shot = col.transform.GetComponent<ShootableRelay>();
            if(shot) {
                shot.TakeDamage(_damage, _playerPoints, false, _powerUpManager);
            }
        }
        hitColliders = Physics.OverlapSphere(transform.position, _radius, _playerLayer);
        foreach(Collider col in hitColliders) {
            PlayerHealth playerHealth = col.transform.GetComponent<PlayerHealth>();
            if(playerHealth) {
                playerHealth.ExplosionDamage(30);
            }
        }
        if(_explodeSound.Clip) {
            _audioManager.PlaySoundAtPoint(_explodeSound, transform.position);
        }
        Destroy(gameObject, 1f);
    }

    public void SetData(PlayerPoints __playerPoints, PowerUpManager __powerUpManager, AudioManager __audioManager, float __damage) {
        _playerPoints = __playerPoints;
        _powerUpManager = __powerUpManager;
        _audioManager = __audioManager;
        if(_damage == 0) _damage = __damage;
    }
}
