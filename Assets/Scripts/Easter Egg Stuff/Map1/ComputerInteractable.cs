using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ComputerInteractable : MonoBehaviour, Interactable
{
    [SerializeField] private AudioManager _audioManager;
    [SerializeField] private Map1EEManager _eeManager;
    [SerializeField] private string _itemToPap;
    [SerializeField] private string _itemToEELockdown;
    [SerializeField] private string _unlockPortalItem;
    [SerializeField] private GameObject _papPortal, _eePortal;
    [SerializeField] private Sound _cacheOpenSound;
    [SerializeField] private TextMeshPro _computerText;
    private bool _switchable = false;
    private bool _selectedEEPortal = true;

    private void Start() {
        _computerText.text = "<color=red>Low Power Mode...</color>";
    }

    public void Interact(PlayerScriptsHandler __playerScripts) {
        if(__playerScripts.GetPlayerInventory().HasItem(_itemToPap) && !__playerScripts.GetPlayerInventory().HasItem(_unlockPortalItem)) {
            _computerText.text = "<color=green>Location Targeted\nTeleporter Active</color>";
            __playerScripts.GetPlayerInventory().AddItem(_unlockPortalItem);
            return;
        }
        if(_eeManager.GetExaustStepComplete() && _eeManager.GetEEState() == Map1EEState.EXHAUST) {
            _audioManager.PlaySound(_cacheOpenSound);
            _eeManager.ProceedFromExaustStep();
        }
        if(__playerScripts.GetPlayerInventory().HasItem(__item: _itemToEELockdown) && _eeManager.GetEEState() == Map1EEState.KEYCARD && !_switchable) {
            _switchable = true;
            SwitchPortal();
        }
        if(_switchable) {
            SwitchPortal();
        }
    }

    public void CoolDown() {
        _computerText.text = "<color=red>Teleporter Cooling Down</color>";
    }

    public void EndCoolDown() {
        _computerText.text = "<color=green>Location Targeted\nTeleporter Active</color>";
    }

    public void CanFinishExaustStep() {
        _computerText.text = "<color=green>Override Program and Open Cache</color>";
    }

    public void OnPower() {
        _computerText.text = "<color=red>Location card required</color>";
    }

    private void SwitchPortal() {
        _selectedEEPortal = !_selectedEEPortal;
        _papPortal.SetActive(!_selectedEEPortal);
        _eePortal.SetActive(_selectedEEPortal);
    }

    public string GetShown(PlayerScriptsHandler __playerScripts, string __interactInput) {
        if(_switchable) return $"{__interactInput} to switch portal location";
        if(__playerScripts.GetPlayerInventory().HasItem(_itemToPap) && !__playerScripts.GetPlayerInventory().HasItem(_unlockPortalItem)) return $"{__interactInput} to Input Location Card";
        if(__playerScripts.GetPlayerInventory().HasItem(__item: _itemToEELockdown) && !_switchable && _eeManager.GetEEState() == Map1EEState.KEYCARD) return $"{__interactInput} to Input Location Card";
        if(!__playerScripts.GetPlayerInventory().HasItem(_itemToPap)) return "Missing location card...";
        if(_eeManager.GetExaustStepComplete() && _eeManager.GetEEState() == Map1EEState.EXHAUST) return $"{__interactInput} to Open Cache";
        return "";
    }

    public void DoneLockDown() {
        _switchable = false;
        _papPortal.SetActive(true);
        _eePortal.SetActive(false);
    }
}
