using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barrier : Buyable
{
    [SerializeField] private float _maxHealth;
    [SerializeField] private float _repairHealth;
    [SerializeField] private GameObject _offMeshLink;
    [SerializeField] private float _health;
    [SerializeField] private float _repairTimer;
    [SerializeField] private MeshRenderer _barrierShown;

    private void Start() {
        _health = _maxHealth;
        _offMeshLink.SetActive(false);
        _repairTimer = 0;
    }

    private void Update() {
        _repairTimer -= Time.deltaTime;
        if(_health <= 0) _barrierShown.material.color = new Color(0, 0, 0, 0.3f);
        else _barrierShown.material.color = new Color(1-(_health/_maxHealth), _health/_maxHealth, 0, 0.3f);
    }

    public float GetHealth() {
        return _health;
    }

    public override void Buy(PlayerScriptsHandler playerScripts)
    {
        if(_health >= _maxHealth) return;
        if(_repairTimer > 0) return;
        _health = Mathf.Min(_maxHealth, _health+_repairHealth);
        _offMeshLink.SetActive(false);
        _repairTimer = 2;
        if(_health <= 0) _barrierShown.material.color = new Color(0, 0, 0, 0.3f);
        else _barrierShown.material.color = new Color(1-(_health/_maxHealth), _health/_maxHealth, 0, 0.3f);
    }

    public override string GetShown(PlayerScriptsHandler playerScripts)
    {
        if(_health >= _maxHealth) return "";
        if(_repairTimer > 0) return $"REPAIR COOL DOWN: {_repairTimer:0.00}!";
        return "Press E To Repair!";
    }

    public void TakeDamage(float damage) {
        _health = Mathf.Max(0, _health-damage);
        if(_health <= 0) _barrierShown.material.color = new Color(0, 0, 0, 0.3f);
        else _barrierShown.material.color = new Color(1-(_health/_maxHealth), _health/_maxHealth, 0, 0.3f);
        if(_health <= 0) {
            _offMeshLink.SetActive(true);
        }
    }

    public void FullRepair() {
        _health = _maxHealth;
        if(_health <= 0) _barrierShown.material.color = new Color(0, 0, 0, 0.3f);
        else _barrierShown.material.color = new Color(1-(_health/_maxHealth), _health/_maxHealth, 0, 0.3f);
        _offMeshLink.SetActive(false);
    }
}
