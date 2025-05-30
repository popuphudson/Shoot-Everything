using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryPickupInteractable : MonoBehaviour, Interactable
{
    [SerializeField] private MeshRenderer _renderer;
    [SerializeField] private PowerManager _powerManager;
    [SerializeField] private string _inventoryItem;
    [SerializeField] private string _pickUpMessage;
    [SerializeField] private string _needsPowerMessage;
    public void Interact(PlayerScriptsHandler __playerScripts)
    {
        if(!_powerManager.IsMapPowered() && _needsPowerMessage != "") return;
        if(__playerScripts.GetPlayerInventory().HasItem(_inventoryItem)) return;
        __playerScripts.GetPlayerInventory().AddItem(_inventoryItem);
        _renderer.enabled = false;
    }

    public string GetShown(PlayerScriptsHandler __playerScripts, string __interactInput)
    {
        if(__playerScripts.GetPlayerInventory().HasItem(_inventoryItem)) return "";
        if(!_powerManager.IsMapPowered() && _needsPowerMessage != "") return _needsPowerMessage;
        return $"{__interactInput} to " + _pickUpMessage;
    }
}
