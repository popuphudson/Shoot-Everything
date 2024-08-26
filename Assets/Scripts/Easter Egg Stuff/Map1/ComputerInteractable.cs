using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComputerInteractable : MonoBehaviour, Interactable
{
    [SerializeField] private Map1EEManager _eeManager;
    [SerializeField] private string _itemToPap;
    [SerializeField] private string _itemToEELockdown;
    [SerializeField] private string _unlockPortalItem;
    [SerializeField] private GameObject _papPortal, _eePortal;
    private bool _switchable = false;
    private bool _selectedEEPortal = false;
    public void Interact(PlayerScriptsHandler __playerScripts) {
        if(__playerScripts.GetPlayerInventory().HasItem(_itemToPap) && !__playerScripts.GetPlayerInventory().HasItem(_unlockPortalItem)) {
            __playerScripts.GetPlayerInventory().AddItem(_unlockPortalItem);
            return;
        }
        if(_eeManager.GetExaustStepComplete() && _eeManager.GetEEState() == Map1EEState.EXHAUST) {
            _eeManager.ProceedFromExaustStep();
        }
        if(__playerScripts.GetPlayerInventory().HasItem(__item: _itemToEELockdown) && !_switchable) {
            _switchable = true;
            SwitchPortal();
        }
        if(_switchable) {
            SwitchPortal();
        }
    }

    private void SwitchPortal() {
        _selectedEEPortal = !_selectedEEPortal;
        _papPortal.SetActive(!_selectedEEPortal);
        _eePortal.SetActive(_selectedEEPortal);
    }

    public string GetShown(PlayerScriptsHandler __playerScripts, string __interactInput) {
        if(_switchable) return $"{__interactInput} to switch portal location";
        if(__playerScripts.GetPlayerInventory().HasItem(_itemToPap) && !__playerScripts.GetPlayerInventory().HasItem(_unlockPortalItem)) return $"{__interactInput} to Input Location Card";
        if(__playerScripts.GetPlayerInventory().HasItem(__item: _itemToEELockdown) && !_switchable) return $"{__interactInput} to Input Location Card";
        if(!__playerScripts.GetPlayerInventory().HasItem(_itemToPap)) return "Missing location card...";
        if(_eeManager.GetExaustStepComplete() && _eeManager.GetEEState() == Map1EEState.EXHAUST) return $"{__interactInput} to Open Cache";
        return "";
    }
}
