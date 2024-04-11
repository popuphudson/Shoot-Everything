using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ZombieSpawner : MonoBehaviour
{
    public static ZombieSpawner Instance;
    [SerializeField] private GameObject _basicZombie;
    [SerializeField] private int _amountForFirstWave;
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
        int chosen = Random.Range(0, _player.CurrentArea.ZombieSpawnPoints.Length);
        Transform toSpawn = _player.CurrentArea.ZombieSpawnPoints[chosen].SpawnPoint;
        Transform chosenBarrier = _player.CurrentArea.ZombieSpawnPoints[chosen].ConnectedBarriers[Random.Range(0, _player.CurrentArea.ZombieSpawnPoints[chosen].ConnectedBarriers.Length)];
        EnemyAI go = Instantiate(_basicZombie, transform).GetComponent<EnemyAI>();
        go.SetTarget(_player.transform);
        go.SetBarrier(chosenBarrier.GetComponent<Barrier>());
        go.SetLevelData(_levelData);
        go.GetComponent<Shootable>().SetHealth(_healthForFirstWave*_wave);
        go.transform.position = toSpawn.position;
    }

    private IEnumerator SpawnZombies() {
        _spawning = true;
        _wave += 1;
        yield return new WaitForSeconds(2);
        _roundText.text = _wave.ToString();
        int additional = (_amountForFirstWave/2)*(_wave-1);
        additional += (additional/2)*Mathf.FloorToInt(_wave/10);
        _leftToSpawn = _amountForFirstWave + additional;
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
