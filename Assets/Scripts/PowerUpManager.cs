using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public enum PowerupType {
    INSTAKILL,
    DOUBLE_POINTS,
    MAX_AMMO,
    CARPENTER,
    NUKE
}

public class PowerUpManager : MonoBehaviour
{
    [SerializeField] private Barrier[] _barriers;
    [SerializeField] private PlayerScriptsHandler _playerScripts;
    [SerializeField] private ZombieSpawner _zombieSpawner;
    [SerializeField] private GameObject[] _powerUpUIs;
    [SerializeField] private TextMeshProUGUI _powerUpPopUpText;
    [SerializeField] private Animator _powerUpPopUpTextAnims;
    private float _doublePointsTimer;
    private float _instaKillTimer;
    private int _killsToNextPowerup;
    private Dictionary<PowerupType, string> _powerUpNames = new Dictionary<PowerupType, string>();
    private int _totalKills;
    private int _powerUpsGotten;

    private void Start() {
        _killsToNextPowerup = Random.Range(10, 20);
        _powerUpNames.Add(PowerupType.MAX_AMMO, "Max Ammo");
        _powerUpNames.Add(PowerupType.INSTAKILL, "Instakill");
        _powerUpNames.Add(PowerupType.CARPENTER, "Carpenter");
        _powerUpNames.Add(PowerupType.NUKE, "Nuke");
        _powerUpNames.Add(PowerupType.DOUBLE_POINTS, "Double Points");
    }

    public int GetKillsToNextPowerup() {
        _totalKills++;
        _killsToNextPowerup--;
        if(_killsToNextPowerup == 0) {
            _powerUpsGotten++;
            _killsToNextPowerup = Random.Range(10+(5*_powerUpsGotten), 30+(7*_powerUpsGotten));
            return 0;
        }
        return _killsToNextPowerup;
    }
    
    void Update()
    {
        _doublePointsTimer = Mathf.Max(_doublePointsTimer-Time.deltaTime, 0);
        _instaKillTimer = Mathf.Max(_instaKillTimer-Time.deltaTime, 0);
        _powerUpUIs[0].SetActive(_doublePointsTimer > 0);
        _powerUpUIs[1].SetActive(_instaKillTimer > 0);
    }

    public bool IsPowerupActive(PowerupType type) {
        switch(type) {
            case PowerupType.INSTAKILL:
                return _instaKillTimer > 0;
            case PowerupType.DOUBLE_POINTS:
                return _doublePointsTimer > 0;
        }
        return false;
    }

    public void ActivatePowerUp(PowerupType type) {
        _powerUpPopUpText.text = _powerUpNames[type];
        _powerUpPopUpTextAnims.Play("Popup");
        switch (type) {
            case PowerupType.INSTAKILL:
                _instaKillTimer += 30;
                break;
            case PowerupType.DOUBLE_POINTS:
                _doublePointsTimer += 30;
                break;
            case PowerupType.CARPENTER:
                foreach(Barrier bar in _barriers) {
                    bar.FullRepair();
                }
                break;
            case PowerupType.MAX_AMMO:
                _playerScripts.GetPlayerGunInventory().RefillAllAmmo();
                break;
            case PowerupType.NUKE:
                _zombieSpawner.KillAll();
                break;
        }
    }

    public int GetTotalKills() {
        return _totalKills;
    }
}
