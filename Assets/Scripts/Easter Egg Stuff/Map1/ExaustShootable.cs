using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExaustShootable : MonoBehaviour
{
    [SerializeField] private Map1EEManager _eeManager;
    [SerializeField] private ParticleSystem _sparkParticles;
    private bool _activated;

    private void Update() {
        if(_eeManager.GetEEState() == Map1EEState.EXHAUST) {
            if(!_activated && !_sparkParticles.isPlaying) _sparkParticles.Play();
            else if(_activated) _sparkParticles.Stop();
        } else {
            _sparkParticles.Stop();
            _activated = false;
        }
    }

    private void OnTriggerEnter(Collider other) {
        if(other.GetComponent<CoilGunStunner>() && _eeManager.GetEEState() == Map1EEState.EXHAUST && !_activated) {
            _activated = true;
            _eeManager.UpdateEEStep();
        }
    }

    public bool GetActive() {
        return _activated;
    }
    
}
