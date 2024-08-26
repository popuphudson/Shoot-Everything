using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking.Types;
using UnityEngine.Rendering.PostProcessing;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private float _orimMaxHealth;
    [SerializeField] private GameObject _reviveEffectVolume;
    [SerializeField] private PlayerScriptsHandler _playerScripts;
    [SerializeField] private Animator _deathScreen;
    [SerializeField] private PostProcessVolume _hurtEffectVolume;
    [SerializeField] private ZombieSpawner _zombieSpawner;
    private PlayerPerks _playerPerks;
    private PlayerLook _playerLook;
    private PlayerMovement _playerMovement;
    private Vignette _vignette;
    private DepthOfField _depthOfField;
    private float _health;
    private float _maxHealth;
    private float _damageTimer;
    private float _preHealth;
    private float _immunityTimer;
    private float _graceTimer;
    private void Start() {
        _playerPerks = _playerScripts.GetPlayerPerks();
        _playerLook = _playerScripts.GetPlayerLook();
        _playerMovement = _playerScripts.GetPlayerMovement();
        _maxHealth = _orimMaxHealth;
        _health = _orimMaxHealth;
        _preHealth = _orimMaxHealth;
        _immunityTimer=0;
        _vignette = _hurtEffectVolume.profile.GetSetting<Vignette>();
        _depthOfField = _hurtEffectVolume.profile.GetSetting<DepthOfField>();
    }

    private void Update() {
        _vignette.intensity.value = _health<=50?1:1-(_health/_maxHealth);
        _depthOfField.enabled.value = _health<=50; 
        if(_playerPerks.HasPerks(Perks.EXTRA_HEALTH)) _maxHealth = _orimMaxHealth+150;
        if(_playerPerks.HasSideMixPerk(Perks.EXTRA_HEALTH) || _playerPerks.HasMainMixPerk(Perks.EXTRA_HEALTH)) _maxHealth = _orimMaxHealth+50;
        else _maxHealth = _orimMaxHealth;
        _damageTimer -= Time.deltaTime;
        if(_graceTimer > 0) _graceTimer -= Time.deltaTime;
        if(_immunityTimer > 0) _immunityTimer -= Time.deltaTime;
        _reviveEffectVolume.SetActive(_immunityTimer>0.5f);
        _hurtEffectVolume.gameObject.SetActive(_immunityTimer<=0.5f);
        if(_immunityTimer < 0 && _zombieSpawner.ZombiesAreForcedIdle()) {
            _zombieSpawner.StopForceIdle();
            _zombieSpawner.SetPhasable(false);
        }
        if(_damageTimer <= 0) {
            float coef = _playerPerks.HasMix(Perks.QUICK_HEAL_LIFE, Perks.EXTRA_HEALTH)?(1-(_health/_maxHealth))*3:1;
            coef *= _playerPerks.HasMix(Perks.QUICK_HEAL_LIFE, Perks.BETTER_RUN)?(_playerMovement.IsRunning()?2.5f:1):1;
            _health = Mathf.Lerp(_preHealth, _maxHealth, -(_damageTimer*coef));
        }
    }

    public void Heal(float __heal) {
        _health += __heal;
        if(_health > _maxHealth) {
            _health = _maxHealth;
        }
    }

    public float GetHealth() {
        return _health;
    }

    public void ExplosionDamage(float __damage) {
        Damage(__damage);
    }

    public void NonLeathalDamage(float __damage) {
        if(_health-__damage > 0) Damage(__damage);
    }

    public void Damage(float __damage) {
        if(_graceTimer > 0) return;
        _graceTimer = 0.5f;
        if(_immunityTimer > 0) {
            _playerLook.AddReversibleRecoil(new Vector2(-2*Random.Range(1f, 2f), 2*Random.Range(-1f, 1f)));
            return;
        }
        _playerLook.AddReversibleRecoil(new Vector2(-10*Random.Range(1f, 2f), 10*Random.Range(-1f, 1f)));
        _health -= __damage;
        _preHealth = _health;
        _damageTimer = 3;
        if(_health <= 0) {
            if(_playerPerks.HasPerks(Perks.QUICK_HEAL_LIFE)) {
                _health = _maxHealth;
                _playerPerks.RemovePerk(Perks.QUICK_HEAL_LIFE);
                _immunityTimer = 10;
                _zombieSpawner.ForceIdleAll();
                _zombieSpawner.SetPhasable(true);
                return;
            }
            _playerScripts.GetPlayerMovement().enabled = false;
            _playerScripts.GetPlayerGunHandler().enabled = false;
            _playerLook.StopPlayerInput();
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
            _deathScreen.Play("Dead");
        }
    }
}
