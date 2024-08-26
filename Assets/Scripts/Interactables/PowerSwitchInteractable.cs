using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerSwitchInteractable : MonoBehaviour, Interactable
{
    [SerializeField] private AudioManager _audioManager;
    [SerializeField] private ParticleSystem _sparks;
    [SerializeField] private Transform _powerSwitch;
    [SerializeField] private PowerManager _powerManager;
    [SerializeField] private Sound _flipSound;
    public void Interact(PlayerScriptsHandler __playerScripts) {
        if(_powerManager.IsMapPowered()) return;
        _powerManager.PowerMap();
        _powerSwitch.rotation = Quaternion.Euler(70, 0, 0);
        _audioManager.PlaySoundAtPoint(_flipSound, transform.position);
        _sparks.Play();
    }

    public string GetShown(PlayerScriptsHandler __playerScripts, string __interactInput) {
        if(_powerManager.IsMapPowered()) return "";
        return $"{__interactInput} To Turn On Power!";
    }
}
