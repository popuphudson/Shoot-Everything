using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] private NavMeshAgent _agent;
    [SerializeField] private Transform _target;
    [SerializeField] private Animator _anims;
    [SerializeField] private LayerMask _solidLayers;
    private Barrier _barrier;
    private bool _ai = true;
    private bool _targetBarrier = false;
    private LevelData _levelData;

    public void SetTarget(Transform target) {
        _target = target;
    }

    public void SetBarrier(Barrier barrier) {
        _barrier = barrier;
    }

    public void SetLevelData(LevelData levelData) {
        _levelData = levelData;
    }

    public bool CanPathToPlayerWithoutBarrierLinks() {
        List<bool> activeLinks = new List<bool>();
        foreach(GameObject link in _levelData.BarrierLinks) {
            activeLinks.Add(link.activeSelf);
            link.SetActive(false);
        }
        NavMeshPath path = new NavMeshPath(); 
        try {
            _agent.CalculatePath(_target.position, path);
        } catch (System.Exception e) {
            return false;
        }
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
            _targetBarrier = _agent.pathStatus == NavMeshPathStatus.PathPartial;
        } else {
            _agent.isStopped = true;
        }
        if(_targetBarrier) {
            _agent.isStopped = false;
            _agent.destination = _barrier.transform.position;
            TryDamageBarrier();
            _targetBarrier = _barrier.GetHealth()>0;
        } else {
            TryDamagePlayer();
        }
    }

    private void TryDamageBarrier() {
        if(Vector3.Distance(_barrier.transform.position, transform.position) <= 2.25f) {
            _anims.Play("Attack");
        }
    }

    private void TryDamagePlayer() {
        if(Vector3.Distance(_target.position, transform.position) <= 2.25f) {
            if(!Physics.Linecast(transform.position, _target.position, _solidLayers)) {
                _anims.Play("Attack");
            }
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
