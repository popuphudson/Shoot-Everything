using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private float _orimMaxHealth;
    [SerializeField] private PlayerPerks _playerPerks;
    [SerializeField] private PlayerLook _playerLook;
    [SerializeField] private GameObject _reviveEffectVolume;
    [SerializeField] private PlayerScriptsHandler _playerScripts;
    [SerializeField] private Animator _deathScreen;
    private float _health;
    private float _maxHealth;
    private float _damageTimer;
    private float _preHealth;
    private float _immunityTimer;
    private void Start() {
        _maxHealth = _orimMaxHealth;
        _health = _orimMaxHealth;
        _preHealth = _orimMaxHealth;
        _immunityTimer=0;
    }

    private void Update() {
        if(_playerPerks.HasPerks(Perks.EXTRA_HEALTH)) _maxHealth = _orimMaxHealth+150;
        else _maxHealth = _orimMaxHealth;
        _damageTimer -= Time.deltaTime;
        if(_immunityTimer > 0) _immunityTimer -= Time.deltaTime;
        _reviveEffectVolume.SetActive(_immunityTimer>0.5f);
        if(_damageTimer <= 0) {
            _health = Mathf.Lerp(_preHealth, _maxHealth, -_damageTimer);
        }
    }

    public void Damage(float damage) {
        if(_immunityTimer > 0) {
            _playerLook.AddReversibleRecoil(new Vector2(-2*Random.Range(1f, 2f), 2*Random.Range(-1f, 1f)));
            return;
        }
        _playerLook.AddReversibleRecoil(new Vector2(-10*Random.Range(1f, 2f), 10*Random.Range(-1f, 1f)));
        _health -= damage;
        _preHealth = _health;
        _damageTimer = 3;
        if(_health <= 0) {
            if(_playerPerks.HasPerks(Perks.QUICK_HEAL_LIFE)) {
                _health = _maxHealth;
                _playerPerks.RemovePerks(Perks.QUICK_HEAL_LIFE);
                _immunityTimer = 5;
                return;
            }
            _playerScripts.GetPlayerMovement().enabled = false;
            _playerScripts.GetPlayerGunInventory().enabled = false;
            _playerLook.StopPlayerInput();
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
            _deathScreen.Play("Dead");
        }
    }
}
