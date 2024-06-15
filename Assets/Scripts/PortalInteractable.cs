using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalInteractable : Interactable
{
    [SerializeField] private PowerManager _powerManager;
    [SerializeField] private string[] _itemsRequired;
    [SerializeField] private bool _needsPower;
    
    public override void Interact(PlayerScriptsHandler __playerScripts)
    {
        if(_needsPower && !_powerManager.IsMapPowered()) return;
        
    }

    public override string GetShown(PlayerScriptsHandler __playerScripts)
    {
        throw new System.NotImplementedException();
    }
}
