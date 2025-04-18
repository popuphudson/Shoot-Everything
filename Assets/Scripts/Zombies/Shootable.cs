using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Shootable : MonoBehaviour
{
    [SerializeField] private Sound _deathSound;
    [SerializeField] private string _notificationName;
    [SerializeField] private float _maxHp;
    [SerializeField] private GameObject _healthBar;
    [SerializeField] private GameObject _pulseBar;
    [SerializeField] private Slider _hpSlider;
    [SerializeField] private Slider _decaySlider;
    [SerializeField] private float _decaySpeed;
    [SerializeField] private int _pointsGiven;
    [SerializeField] private EnemyAI _enemyAI;
    [SerializeField] private GameObject _powerUpPrefab;
    public UnityEvent DespawnEvent;
    private AudioManager _audioManager;
    private float _prevDecay;
    private float _decayTimer;
    private float _hp;
    private bool _showing = false;
    private void Start() {
        _hp = _maxHp;
        if(_hpSlider) {
            _hpSlider.maxValue = _maxHp;
            _hpSlider.value = _hp;
            _decaySlider.maxValue = _maxHp;
            _decaySlider.value = _hp;
            _prevDecay = _decaySlider.value;
        }
        _enemyAI.SetAudioManager(_audioManager);
    }

    public void SetAudioManager(AudioManager __audioManager) {
        _audioManager = __audioManager;
    }

    public void SetHealth(float __hp) {
        _hp = __hp;
        _maxHp = __hp;
    }

    public void ShowHealth() {
        _showing = true;
        _healthBar.gameObject.SetActive(true);
    }

    public void HideHealth() {
        _healthBar.gameObject.SetActive(false);
    }

    private void Update() {
        if(_hpSlider) {
            _decayTimer += Time.deltaTime/_decaySpeed;
            _decaySlider.value = Mathf.Lerp(_prevDecay, _hp, Mathf.Max(0, _decayTimer));
            _pulseBar.transform.localScale = new Vector3(_pulseBar.transform.localScale.x, (_prevDecay-_hp)==0?0:((_decaySlider.value-_hp)/(_prevDecay-_hp)), _pulseBar.transform.localScale.z);
        }
        if(_showing) {
            _showing = false;
        } else {
            HideHealth();
        }
    }

    public void Despawn() {
        _enemyAI.Killed();
        _audioManager.PlaySoundAtPoint(_deathSound, transform.position);
        gameObject.SetActive(false);
        DespawnEvent.Invoke();
        Destroy(gameObject, 1f);
    }

    public void TakeDamage(float __damage, PlayerPoints __playerPoints, float __pointmul, PowerUpManager __powerUpManager) {
        _prevDecay = _decaySlider.value;
        _hp -= __damage;
        if(__damage < 0) _hp = 0;
        if(_hpSlider) {
            _decayTimer = 0;
            _hpSlider.value = _hp;
        }
        if(_hp <= 0) {
            if(_notificationName != "" && _enemyAI) {
                if(__playerPoints) __playerPoints.AddPoints(Mathf.CeilToInt(_pointsGiven*__pointmul));
                if(_enemyAI.CanPathToPlayerWithoutBarrierLinks()) {
                    if(__powerUpManager.GetKillsToNextPowerup() == 0) {
                        Debug.Log("Powerup Spawned");
                        PowerUp go = Instantiate(_powerUpPrefab, transform.position, Quaternion.identity).GetComponent<PowerUp>();
                        go.SetPowerUpManager(__powerUpManager);
                        PowerupType powerupType = (PowerupType)System.Enum.GetValues(typeof(PowerupType)).GetValue(Random.Range(0, System.Enum.GetValues(typeof(PowerupType)).Length));
                        go.transform.GetChild(0).GetChild(0).GetComponent<TextMeshPro>().text = powerupType.ToString()[0].ToString().ToUpper() + powerupType.ToString().Substring(1).Replace("_", " ").ToLower();
                        go.SetPowerUpType(powerupType);
                    }
                }
            }
            _enemyAI.Killed();
            _audioManager.PlaySoundAtPoint(_deathSound, transform.position);
            gameObject.SetActive(false);
            Destroy(gameObject, 1f);
        }
    }

    public float GetHealth() {
        return _hp;
    }
    public float GetMaxHealth() {
        return _maxHp;
    }
}
