using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] private Sound _zombieNormalHit, _zombieQuickHit, _zombieGroan;
    [SerializeField] private NavMeshAgent _agent;
    [SerializeField] private Transform _target;
    [SerializeField] private Animator _anims;
    [SerializeField] private LayerMask _solidLayers;
    private AudioManager _audioManager;
    private BarrierInteractable _barrier;
    private bool _ai = true;
    private bool _targetBarrier = false;
    private LevelData _levelData;
    private bool _attacking = false;
    private bool _forceRunning = false;
    private bool _running = false;
    private bool _randomIdle = false;
    private Vector3 _randomPoint;
    private bool _forceIdle;
    private PlayerMovement _targetMovement;
    private bool _quickAttack;
    private AudioSource _groanSound;

    public void Killed() {
        if(_groanSound) {
            Destroy(_groanSound);
        }
    }

    public void RetargetBarriers() {
        if(_agent.isOnNavMesh && CanPathToPlayerWithoutBarrierLinks()) _targetBarrier = false;
        else _targetBarrier = true;
    }

    public void SetAudioManager(AudioManager __audioManager) {
        _audioManager = __audioManager;
    }

    private void Start() {
        if(_barrier) _targetBarrier = true;
    }

    public void ForceRun() {
        _forceRunning = true;
        SetRunning();
    }

    public void SetRunning() {
        _agent.speed = 5;
        _agent.acceleration = 16;
        _agent.angularSpeed = 360;
        _running = true;
        _anims.SetBool("running", true);
    }

    public void SetWalking() {
        _agent.speed = 1;
        _agent.acceleration = 8;
        _agent.angularSpeed = 180;
        _running = false;
        _anims.SetBool("running", false);
    }

    public void StopAttacking() {
        _attacking = false;
    }

    private void UpdateAnimationMoving() {
        _anims.speed = 1;
        if(_running) _anims.CrossFade("Run", 0.1f, 0);
        else _anims.CrossFade("Walk", 0.1f, 0);
    }

    public void SetTarget(Transform __target) {
        _target = __target;
        _targetMovement = __target.GetComponent<PlayerMovement>();
    }

    public void SetBarrier(BarrierInteractable __barrier) {
        _barrier = __barrier;
    }

    public void SetLevelData(LevelData __levelData) {
        _levelData = __levelData;
    }

    public bool CanPathToPlayerWithoutBarrierLinks() {
        if(!_agent.isOnNavMesh) return false;
        List<bool> activeLinks = new List<bool>();
        foreach(GameObject link in _levelData.BarrierLinks) {
            activeLinks.Add(link.activeSelf);
            link.SetActive(false);
        }
        NavMeshPath path = new NavMeshPath(); 
        _agent.CalculatePath(_target.position, path);
        bool pathStatus = path.status == NavMeshPathStatus.PathComplete;
        for(int i = 0; i < _levelData.BarrierLinks.Length; i++) {
            _levelData.BarrierLinks[i].SetActive(activeLinks[i]);
        }
        return pathStatus;
    }

    public void ForceIdle() {
        _randomPoint = RandomPlace();
        _forceIdle = true;
        UpdateAnimationMoving();
    }

    public void StopIdle() {
        _forceIdle = false;
    }

    private void EnsurePathingToTarget() {
        NavMeshPath path = new NavMeshPath(); 
        _agent.CalculatePath(_target.position, path);
        if(path.status != NavMeshPathStatus.PathComplete) {
            NavMeshHit hit;
            if(!_randomIdle && !_forceIdle && NavMesh.SamplePosition(_target.position, out hit, 2f, NavMesh.AllAreas)) _agent.destination = hit.position;
            else if(!_randomIdle && !_forceIdle) {
                _randomPoint = RandomPlace();
                _randomIdle = true;
            }
        } else {
            _randomIdle = false;
        }
    }

    private void LockOnTarget() {
        _agent.isStopped = false;
        _agent.destination = _target.position;
        if(_agent.isOnNavMesh) EnsurePathingToTarget();
    }

    void Update()
    {
        if(Vector3.Distance(_target.position, transform.position) > 2.25f && _ai && !_targetBarrier) {
            LockOnTarget();
        } else if(!_forceIdle) {
            _agent.isStopped = true;
        }
        if(_randomIdle || _forceIdle) {
            _agent.isStopped = false;
            _agent.destination = new Vector3(_randomPoint.x, transform.position.y, _randomPoint.z);
            if(_agent.isOnNavMesh) {
                NavMeshPath path = new NavMeshPath(); 
                _agent.CalculatePath(new Vector3(_randomPoint.x, transform.position.y, _randomPoint.z), path);
                _randomIdle = Vector3.Distance(new Vector3(_randomPoint.x, transform.position.y, _randomPoint.z), transform.position) > 0.25f && path.status == NavMeshPathStatus.PathComplete;
                if(_forceIdle && !_randomIdle) {
                    _randomPoint = RandomPlace();
                }
            }
            if(!_running && !_forceRunning) { SetRunning(); UpdateAnimationMoving(); }
        } else if(_running && !_forceRunning) { SetWalking(); UpdateAnimationMoving(); }
        bool preAttacking = _attacking;
        if(_targetBarrier) {
            _agent.isStopped = false;
            _agent.destination = _barrier.transform.position;
            TryDamageBarrier();
            _targetBarrier = _barrier.GetHealth()>0;
        } else {
            if(!_forceIdle) TryDamagePlayer();
        }
        if(preAttacking != _attacking) {
            if(_attacking && !_forceIdle)  {
                if(_running) _anims.speed = 1.4f;
                else _anims.speed = 1;
                if(_targetBarrier) {
                    _anims.CrossFade("Attack", 0.1f, 0);
                } else {
                    if(_targetMovement.GetMoveVelocity().magnitude >= 7f) {
                        _anims.CrossFade("Attack", 0.1f, 0, 18f/79f);
                        _quickAttack = true;
                    } else {
                        _anims.CrossFade("Attack", 0.1f, 0);
                        _quickAttack = false;
                    }
                }
            } else {
                UpdateAnimationMoving();
            }
        }
    }

    private void TryDamageBarrier() {
        if(Vector3.Distance(_barrier.transform.position, transform.position) <= 2.25f && !_anims.IsInTransition(0) && !_attacking) {
            _groanSound = _audioManager.PlaySoundAtPoint(_zombieGroan, transform.position);
            _attacking = true;
        } else if(Vector3.Distance(_barrier.transform.position, transform.position) > 2.25f) {
            _attacking = false;
            if(_groanSound) {
                Destroy(_groanSound);
            }
        }
    }

    private void TryDamagePlayer() {
        if(Vector3.Distance(_target.position, transform.position) <= 2.25f && !_anims.IsInTransition(0) && !_attacking) {
            if(!Physics.Linecast(transform.position, _target.position, _solidLayers)) {
                _groanSound = _audioManager.PlaySoundAtPoint(_zombieGroan, transform.position);
                _attacking = true;
            } else {
                _attacking = false;
                if(_groanSound) {
                    Destroy(_groanSound);
                }
            }
        } else if(Vector3.Distance(_target.position, transform.position) > 2.25f) {
            _attacking = false;
            if(_groanSound) {
                Destroy(_groanSound);
            }
        }
    }

    public void CauseDamage() {
        if(_targetBarrier) {
            if(Vector3.Distance(_barrier.transform.position, transform.position) > 2.25f) return;
            _barrier.TakeDamage(50);
            _audioManager.PlaySoundAtPoint(_zombieNormalHit, transform.position);
            if(_groanSound) {
                Destroy(_groanSound);
            }
        } else {
            if(Vector3.Distance(_target.position, transform.position) > 2.5f) return;
            PlayerHealth playerHealth = _target.GetComponent<PlayerHealth>();
            if(!playerHealth) return;
            if(_quickAttack) { 
                _audioManager.PlaySoundAtPoint(_zombieQuickHit, transform.position);
                playerHealth.Damage(20f); 
            } else { 
                _audioManager.PlaySoundAtPoint(_zombieNormalHit, transform.position);
                playerHealth.Damage(50f); 
            }
            if(_groanSound) {
                Destroy(_groanSound);
            }
        }
    }

    public void SetAi(bool set) {
        _ai = set;
    }

    private Vector3 RandomPlace() {
        for (int i = 0; i < 30; i++) {
            Vector3 randomCentre = _levelData.Areas[Random.Range(0, _levelData.Areas.Length)].transform.position;
            Vector3 randomPoint = Random.insideUnitSphere;
            randomPoint.x *= 5f;
            randomPoint.z *= 5f;
            randomPoint += randomCentre;
            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPoint, out hit, 3.0f, NavMesh.AllAreas)) {
                if(_agent.isOnNavMesh) {
                    NavMeshPath path = new NavMeshPath();
                    _agent.CalculatePath(hit.position, path);
                    if(path.status == NavMeshPathStatus.PathComplete) return hit.position;
                } else return hit.position;
            }
        }
        return transform.position;
    }
}