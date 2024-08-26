using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PortalSound {
    NONE,
    START,
    END
}

public class PortalInteractable : MonoBehaviour, Interactable
{
    [SerializeField] private AudioManager _audioManager;
    [SerializeField] private PowerManager _powerManager;
    [SerializeField] private string[] _itemsRequired;
    [SerializeField] private string[] _missingItemMessages;
    [SerializeField] private bool _needsPower;
    [SerializeField] private Vector3[] _locations;
    [SerializeField] private Vector3[] _eulerRotations;
    [SerializeField] private PortalSound[] _portalSoundQueues;
    [SerializeField] private float[] _times;
    [SerializeField] private bool[] _locksMovement;
    [SerializeField] private float _coolDownTime;
    [SerializeField] private Sound _portalStartSound, _portalEndSound;
    [SerializeField] private Triggerable[] _teleportTriggerable;
    [SerializeField] private Triggerable[] _resetTriggerable;
    [SerializeField] private string _customTeleportText;
    float _coolDownTimer;
    private bool _reset = true;
    
    private void Start() {
        _reset = true;
    }

    public void Interact(PlayerScriptsHandler __playerScripts)
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
        foreach(Triggerable trigger in _teleportTriggerable) {
            trigger.Trigger();
        }
        _coolDownTimer = _coolDownTime;
        _reset = false;
    }

    public string GetShown(PlayerScriptsHandler __playerScripts, string __interactInput)
    {
        if(_needsPower && !_powerManager.IsMapPowered()) return "Needs power...";
        for(int i = 0; i<_itemsRequired.Length; i++) {
            if(!__playerScripts.GetPlayerInventory().HasItem(_itemsRequired[i])) return _missingItemMessages[i];
        }
        if(_coolDownTimer > 0) return "On cooldown";
        if(_customTeleportText == "") return $"{__interactInput} to teleport";
        else return $"{__interactInput} {_customTeleportText}";
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
        if(_coolDownTimer <= 0) {
            if(!_reset) {
                foreach(Triggerable trigger in _resetTriggerable) {
                    trigger.Trigger();
                }
                _reset = true;
            }
            return;
        }
        _coolDownTimer -= Time.deltaTime;
    }
}
