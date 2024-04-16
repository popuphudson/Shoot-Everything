using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.ProBuilder.MeshOperations;

public class ZombieSpawner : MonoBehaviour
{
    public static ZombieSpawner Instance;
    [SerializeField] private GameObject _basicZombie;
    [SerializeField] private int _zombiesForFirstWave;
    [SerializeField] private float _healthForFirstWave;
    [SerializeField] private int _maxZombiesAliveAtOnce;
    [SerializeField] private PlayerMovement _player;
    [SerializeField] private TextMeshProUGUI _roundText;
    [SerializeField] private LevelData _levelData;
    [SerializeField] private PowerUpManager _powerUpManager;
    private int _wave = 0;
    private bool _spawning = false;
    private int _leftToSpawn;
    private float _timeBetweenSpawns;
    private float _spawnTimer;

    private void Awake() {
        if(Instance != null) {
            Destroy(gameObject);
        }
        Instance = this;
    }

    private void Update() {
        _spawnTimer -= Time.deltaTime;
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
        Transform toSpawn = SpawnPoints[chosen].SpawnPoint;
        Transform chosenBarrier = SpawnPoints[chosen].ConnectedBarriers[Random.Range(0, SpawnPoints[chosen].ConnectedBarriers.Length)];
        EnemyAI go = Instantiate(_basicZombie, toSpawn.position, Quaternion.identity).GetComponent<EnemyAI>();
        go.transform.parent = transform;
        go.SetTarget(_player.transform);
        go.SetBarrier(chosenBarrier.GetComponent<Barrier>());
        go.SetLevelData(_levelData);
        float health = _healthForFirstWave+(100*Mathf.Min(_wave-1, 8));
        if(_wave > 9) {
            health *= Mathf.Pow(1.1f, _wave-9);
        }
        go.GetComponent<Shootable>().SetHealth(health);
    }

    private IEnumerator SpawnZombies() {
        _spawning = true;
        _wave += 1;
        yield return new WaitForSeconds(4);
        _roundText.text = _wave.ToString();
        if(_wave < 10) _leftToSpawn = _zombiesForFirstWave+(2*(_wave-1));
        else _leftToSpawn = Mathf.CeilToInt((0.000058f*Mathf.Pow(_wave, 3))+(0.074032f*Mathf.Pow(_wave, 2))+(0.718119f*_wave)+14.738699f);
        _timeBetweenSpawns = 20/_leftToSpawn;
    }

    public void KillAll() {
        for(int i = 0; i < transform.childCount; i++) {
            transform.GetChild(i).gameObject.GetComponent<Shootable>().TakeDamage(-1, _player.GetComponent<PlayerPoints>(), 1, _powerUpManager);
        }
    }

    public int GetRound() {
        return _wave;
    }
}
