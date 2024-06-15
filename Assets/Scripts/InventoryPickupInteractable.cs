using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryPickupInteractable : Interactable
{
    [SerializeField] private PowerManager _powerManager;
    [SerializeField] private string _inventoryItem;
    [SerializeField] private string _pickUpMessage;
    [SerializeField] private string _needsPowerMessage;
    public override void Interact(PlayerScriptsHandler __playerScripts)
    {
        if(!_powerManager.IsMapPowered() && _needsPowerMessage != "") return;
        if(!__playerScripts.GetPlayerInventory().HasItem(_inventoryItem)) return;
        __playerScripts.GetPlayerInventory().AddItem(_inventoryItem);
        gameObject.SetActive(false);
    }

    public override string GetShown(PlayerScriptsHandler __playerScripts)
    {
        if(__playerScripts.GetPlayerInventory().HasItem(_inventoryItem)) return "";
        if(!_powerManager.IsMapPowered() && _needsPowerMessage != "") return _needsPowerMessage;
        return _pickUpMessage;
    }
}
