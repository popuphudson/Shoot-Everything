using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] private NavMeshAgent _agent;
    [SerializeField] private Transform _target;
    [SerializeField] private Animator _anims;
    [SerializeField] private LayerMask _solidLayers;
    private BarrierInteractable _barrier;
    private bool _ai = true;
    private bool _targetBarrier = false;
    private LevelData _levelData;
    private bool _attacking = false;
    private bool _running = false;

    public void SetRunning() {
        _agent.speed = 5;
        _agent.acceleration = 16;
        _agent.angularSpeed = 360;
        _running = true;
        _anims.SetBool("running", true);
    }

    public void SetTarget(Transform __target) {
        _target = __target;
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

    void Update()
    {
        if(Vector3.Distance(_target.position, transform.position) > 2.25f && _ai && !_targetBarrier) {
            _agent.isStopped = false;
            _agent.destination = _target.position;
            if(_agent.isOnNavMesh) {
                NavMeshPath path = new NavMeshPath(); 
                _agent.CalculatePath(_target.position, path);
                _targetBarrier = _agent.pathStatus == NavMeshPathStatus.PathPartial;
            } 
        } else {
            _agent.isStopped = true;
        }
        bool preAttacking = _attacking;
        if(_targetBarrier) {
            _agent.isStopped = false;
            _agent.destination = _barrier.transform.position;
            TryDamageBarrier();
            _targetBarrier = _barrier.GetHealth()>0;
        } else {
            TryDamagePlayer();
        }
        if(preAttacking != _attacking) {
            if(_attacking)  {
                if(_running) _anims.speed = 1.4f;
                else _anims.speed = 1;
                _anims.CrossFade("Attack", 0.1f, 0);
            } else {
                _anims.speed = 1;
                if(_running) _anims.CrossFade("Run", 0.1f, 0);
                else _anims.CrossFade("Walk", 0.1f, 0);
            }
        }
    }

    private void TryDamageBarrier() {
        if(Vector3.Distance(_barrier.transform.position, transform.position) <= 2.25f && !_anims.IsInTransition(0) && !_attacking) {
            _attacking = true;
        } else if(Vector3.Distance(_barrier.transform.position, transform.position) > 2.25f) {
            _attacking = false;
        }
    }

    private void TryDamagePlayer() {
        if(Vector3.Distance(_target.position, transform.position) <= 2.25f && !_anims.IsInTransition(0) && !_attacking) {
            if(!Physics.Linecast(transform.position, _target.position, _solidLayers)) {
                _attacking = true;
            } else {
                _attacking = false;
            }
        } else if(Vector3.Distance(_target.position, transform.position) > 2.25f) {
            _attacking = false;
        }
    }

    public void CauseDamage() {
        if(_targetBarrier) {
            if(Vector3.Distance(_barrier.transform.position, transform.position) > 2.25f) return;
            _barrier.TakeDamage(50);
        } else {
            if(Vector3.Distance(_target.position, transform.position) > 2.5f) return;
            PlayerHealth playerHealth = _target.GetComponent<PlayerHealth>();
            if(!playerHealth) return;
            playerHealth.Damage(50);
        }
    }

    public void SetAi(bool set) {
        _ai = set;
    }
}
