using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PortalSound {
    NONE,
    START,
    END
}

public class PortalInteractable : Interactable
{
    [SerializeField] private AudioManager _audioManager;
    [SerializeField] private PowerManager _powerManager;
    [SerializeField] private string[] _itemsRequired;
    [SerializeField] private bool _needsPower;
    [SerializeField] private Vector3[] _locations;
    [SerializeField] private Vector3[] _eulerRotations;
    [SerializeField] private PortalSound[] _portalSoundQueues;
    [SerializeField] private float[] _times;
    [SerializeField] private bool[] _locksMovement;
    [SerializeField] private float _coolDownTime;
    [SerializeField] private string _missingItemMessage;
    [SerializeField] private Sound _portalStartSound, _portalEndSound;
    float _coolDownTimer;
    
    public override void Interact(PlayerScriptsHandler __playerScripts)
    {
        if(_needsPower && !_powerManager.IsMapPowered()) return;
        if(!__playerScripts.GetPlayerInventory().HasItems(_itemsRequired)) return;
        if(_coolDownTimer > 0) return;
        if(_locations.Length == 1 && _times[0] == -1) {
            __playerScripts.GetPlayerMovement().Teleport(_locations[0]);
            __playerScripts.GetPlayerLook().SetRotation(_eulerRotations[0]);
        } else {
            StartCoroutine(Teleportations(__playerScripts));
        }
        _coolDownTimer = _coolDownTime;
    }

    public override string GetShown(PlayerScriptsHandler __playerScripts)
    {
        if(_needsPower && !_powerManager.IsMapPowered()) return "Needs power...";
        if(!__playerScripts.GetPlayerInventory().HasItems(_itemsRequired)) return _missingItemMessage;
        if(_coolDownTimer > 0) return "On cooldown";
        return "E to teleport";
    }

    IEnumerator Teleportations(PlayerScriptsHandler __playerScripts) {
        AudioSource prevSound = null;
        for(int i = 0; i < _locations.Length; i++) {
            __playerScripts.GetPlayerMovement().Teleport(_locations[i]);
            __playerScripts.GetPlayerLook().SetRotation(_eulerRotations[i]);
            if(_locksMovement[i]) __playerScripts.GetPlayerMovement().Freeze();
            if(prevSound) {
                Destroy(prevSound);
            }
            switch(_portalSoundQueues[i]) {
                case PortalSound.START:
                    prevSound = _audioManager.PlaySound(_portalStartSound);
                    break;
                case PortalSound.END:
                    prevSound = _audioManager.PlaySound(_portalEndSound);
                    break;
            }
            yield return new WaitForSeconds(_times[i]);
            __playerScripts.GetPlayerMovement().UnFreeze();
        }
    }

    private void Update() {
        if(_coolDownTimer <= 0) return;
        _coolDownTimer -= Time.deltaTime;
    }
}
