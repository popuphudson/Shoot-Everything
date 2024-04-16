using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoilGunStunner : MonoBehaviour
{
    [SerializeField] private LayerMask _enemySplashLayer;
    [SerializeField] private float _stayTime = 5;
    private PlayerPoints _playerPoints;
    private PowerUpManager _powerUpManager;

    private void Start() {
        Destroy(gameObject, _stayTime);
    }

    public void SetData(PlayerPoints playerPoints, PowerUpManager powerUpManager) {
        _playerPoints = playerPoints;
        _powerUpManager = powerUpManager;
    }

    void Update()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 2f, _enemySplashLayer);
        foreach(Collider col in hitColliders) {
            ShootableRelay shot = col.transform.GetComponent<ShootableRelay>();
            if(shot) {
                shot.TakeDamage(shot.GetShootableMaxHealth()*Time.deltaTime, _playerPoints, false, _powerUpManager);
            }
        }
    }
}
