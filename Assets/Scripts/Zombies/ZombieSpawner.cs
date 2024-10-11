using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class ZombieSpawner : MonoBehaviour 
{
    public static ZombieSpawner Instance;
    [SerializeField] private AudioManager _audioManager; 
    [SerializeField] private Sound _roundChangeSound;
    [SerializeField] private Sound[] _zombieAmbientSounds;
    [SerializeField] private GameObject _basicZombie;
    [SerializeField] private int _zombiesForFirstWave;
    [SerializeField] private float _healthForFirstWave;
    [SerializeField] private int _maxZombiesAliveAtOnce;
    [SerializeField] private PlayerMovement _player;
    [SerializeField] private TextMeshProUGUI _roundText;
    [SerializeField] private Animator _roundTextAnims;
    [SerializeField] private LevelData _levelData;
    [SerializeField] private PowerUpManager _powerUpManager;
    [SerializeField] private UnityEvent _onNextWave;
    private int _wave = 0;
    private bool _spawning = false;
    private int _leftToSpawn;
    private float _timeBetweenSpawns;
    private float _spawnTimer;
    private bool _forcedIdle;
    private float _ambientTimer;
    private bool _spawnable;

    public bool ZombiesAreForcedIdle() {
        return _forcedIdle;
    }

    private void Awake() {
        if(Instance != null) {
            Destroy(gameObject);
        }
        Instance = this;
        _ambientTimer = Random.Range(3, 5);
        _spawnable = true;
    }

    private void PlayAmbientZombieNoise() {
        _ambientTimer = Random.Range(3, 5);
        if(transform.childCount == 0) return;
        for(int i = 0; i < Mathf.Max(1, Mathf.FloorToInt(transform.childCount/10)); i++) {
            int choice = Random.Range(0, transform.childCount);
            _audioManager.PlaySoundAtPoint(_zombieAmbientSounds[Random.Range(0, _zombieAmbientSounds.Length)], transform.GetChild(choice).position);
        }
    }

    private void Update() {
        if(!_spawnable) return;
        _spawnTimer -= Time.deltaTime;
        _ambientTimer -= Time.deltaTime;
        if(_ambientTimer <= 0) {
            PlayAmbientZombieNoise();
        }
        if(_leftToSpawn > 0 && transform.childCount < _maxZombiesAliveAtOnce && _spawnTimer < 0) {
            SpawnBasicZombie();
            _leftToSpawn--;
            _spawning = _leftToSpawn>0;
            _spawnTimer = _timeBetweenSpawns;
        }
        if(transform.childCount == 0 && !_spawning) {
            StartCoroutine(SpawnZombies());
        }
    }

    private void SpawnBasicZombie() {
        List<ZombieSpawnPoint> SpawnPoints = new List<ZombieSpawnPoint>();
        foreach(ZombieSpawnPoint spawnPoint in _player.CurrentArea.ZombieSpawnPoints) {
            SpawnPoints.Add(spawnPoint);
        }
        foreach(AreaDataLink linkedArea in _player.CurrentArea.AreaLinks) {
            if(linkedArea.LinkEnabled) {
                foreach(ZombieSpawnPoint spawnPoint in linkedArea.AreaData.ZombieSpawnPoints) {
                    SpawnPoints.Add(spawnPoint);
                }
            }
        }
        int chosen = Random.Range(0, SpawnPoints.Count);
        Transform toSpawn = SpawnPoints[0].SpawnPoint;
        try {
            toSpawn = SpawnPoints[chosen].SpawnPoint;
        } catch(System.IndexOutOfRangeException) {
            Debug.LogError($"{chosen} isn't within the confines of 0-{SpawnPoints.Count-1}!");
        }
        Transform chosenBarrier = SpawnPoints[chosen].ConnectedBarriers.Length>0?SpawnPoints[chosen].ConnectedBarriers[Random.Range(0, SpawnPoints[chosen].ConnectedBarriers.Length)]:null;
        EnemyAI enemyAI = Instantiate(_basicZombie, toSpawn.position, Quaternion.identity).GetComponent<EnemyAI>();
        enemyAI.transform.parent = transform;
        enemyAI.SetTarget(_player.transform);
        if(chosenBarrier) enemyAI.SetBarrier(chosenBarrier.GetComponent<BarrierInteractable>());
        enemyAI.SetLevelData(_levelData);
        if(ZombieGonnaRun()) enemyAI.ForceRun();
        float health = _healthForFirstWave+(100*Mathf.Min(_wave-1, 8));
        if(_wave > 9) {
            health *= Mathf.Pow(1.125f, _wave-9);
        }
        enemyAI.GetComponent<Shootable>().SetHealth(health);
        enemyAI.GetComponent<Shootable>().SetAudioManager(_audioManager);
    }

    private IEnumerator SpawnZombies() {
        _spawning = true;
        _wave += 1;
        _roundTextAnims.Play("Start Round");
        _audioManager.PlaySound(_roundChangeSound);
        _onNextWave.Invoke();
        yield return new WaitForSeconds(0.3f);
        _roundText.text = _wave.ToString();
        if(_wave==1) yield return new WaitForSeconds(4);
        else yield return new WaitForSeconds(8);
        if(_wave < 10) _leftToSpawn = _zombiesForFirstWave+(2*(_wave-1));
        else _leftToSpawn = Mathf.CeilToInt((0.000058f*Mathf.Pow(_wave, 3))+(0.074032f*Mathf.Pow(_wave, 2))+(0.718119f*_wave)+14.738699f);
        _timeBetweenSpawns = 20/Mathf.Min(_leftToSpawn, 24);
    }

    private bool ZombieGonnaRun() {
        float chance = Mathf.Max((float)(_wave-4)/10, 0);
        if(chance >= 1) return true;
        float chosen = Random.Range(0f, 1f);
        return chosen<=chance;
    }

    public void KillAll() {
        for(int i = 0; i < transform.childCount; i++) {
            transform.GetChild(i).gameObject.GetComponent<Shootable>().TakeDamage(-1, null, 1, _powerUpManager);
        }
    }

    public void RespawnAll() {
        for(int i = 0; i < transform.childCount; i++) {
            transform.GetChild(i).gameObject.GetComponent<Shootable>().TakeDamage(-1, null, 1, _powerUpManager);
            _leftToSpawn++;
        }
    }

    public void SetPhasable(bool __phasable) {
        for(int i = 0; i < transform.childCount; i++) {
            transform.GetChild(i).gameObject.GetComponent<CapsuleCollider>().enabled = !__phasable;
        }
    }

    public void ForceIdleAll() {
        for(int i = 0; i < transform.childCount; i++) {
            transform.GetChild(i).gameObject.GetComponent<EnemyAI>().ForceIdle();
        }
        _forcedIdle = true;
    }

    public void StopForceIdle() {
        for(int i = 0; i < transform.childCount; i++) {
            transform.GetChild(index: i).gameObject.GetComponent<EnemyAI>().StopIdle();
        }
        _forcedIdle = false;
    }

    public int GetRound() {
        return _wave;
    }

    public void Pause() {
        _spawnable = false;
    }

    public void Resume() {
        _spawnable = true;
    }
}
