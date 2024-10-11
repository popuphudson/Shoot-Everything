using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    [SerializeField] private float _damage;
    [SerializeField] private GameObject _explosion;
    private AudioManager _audioManager;
    private PlayerPoints _playerPoints;
    private PowerUpManager _powerUpManager;
    private void Start() {
        Invoke("Explode", 2f);
    }

    private void Explode() {
        Explosion explosion = Instantiate(_explosion, transform.position, Quaternion.identity).GetComponent<Explosion>();
        explosion.SetData(_playerPoints, _powerUpManager, _audioManager, _damage);
        Destroy(gameObject);
    }

    public void SetData(PlayerPoints __playerPoints, PowerUpManager __powerUpManager, AudioManager __audioManager) {
        _playerPoints = __playerPoints;
        _powerUpManager = __powerUpManager;
        _audioManager = __audioManager;
    }
}
