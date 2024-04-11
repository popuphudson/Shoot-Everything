using UnityEngine;
using UnityEngine.UI;

public class Shootable : MonoBehaviour
{
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
    }

    public void SetHealth(float hp) {
        _hp = hp;
        _maxHp = hp;
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

    public void TakeDamage(float damage, PlayerPoints playerPoints, float pointmul, PowerUpManager powerUpManager) {
        _prevDecay = _decaySlider.value;
        _hp -= damage;
        if(damage < 0) _hp = 0;
        if(_hpSlider) {
            _decayTimer = 0;
            _hpSlider.value = _hp;
        }
        if(_hp <= 0) {
            if(_notificationName != "" && _enemyAI) {
                playerPoints.AddPoints((int)(_pointsGiven*pointmul));
                if(_enemyAI.CanPathToPlayerWithoutBarrierLinks()) {
                    if(powerUpManager.GetKillsToNextPowerup() == 0) {
                        PowerUp go = Instantiate(_powerUpPrefab, transform.position, Quaternion.identity).GetComponent<PowerUp>();
                        go.SetPowerUpManager(powerUpManager);
                        PowerupType powerupType = (PowerupType)System.Enum.GetValues(typeof(PowerupType)).GetValue(Random.Range(0, System.Enum.GetValues(typeof(PowerupType)).Length));
                        go.SetPowerUpType(powerupType);
                    }
                }
            }
            gameObject.SetActive(false);
            Destroy(gameObject, 1f);
        }
    }
}
