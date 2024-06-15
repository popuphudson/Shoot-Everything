using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerkBuyable : Interactable
{
    [SerializeField] private int _cost;
    [SerializeField] private PowerManager _powerManager;
    [SerializeField] private string _name;
    [SerializeField] private Perks _perks;
    [SerializeField] private int _buylimit;
    [SerializeField] private MeshRenderer _render;
    [SerializeField] private bool _needsPower;
    private int _bought = 0;

    private void Start() {
        _bought = 0;
    }

    public override void Interact(PlayerScriptsHandler __playerScripts)
    {
        if(!_powerManager.IsMapPowered() && _needsPower) return;
        if(_buylimit != -1 && _bought > _buylimit) return;
        if(__playerScripts.GetPlayerPerks().GetNumberOfPerks() >= 5) return;
        if(__playerScripts.GetPlayerPoints().GetPoints() < _cost) return;
        PlayerPerks playerPerks = __playerScripts.GetPlayerPerks();
        if(playerPerks.HasPerks(_perks)) return;
        __playerScripts.GetPlayerPoints().RemovePoints(_cost);
        playerPerks.AddPerks(_perks);
        if(_buylimit != -1) _bought++;
        if(_buylimit != -1 && _bought > _buylimit) {
            _render.enabled = false;
        }
    }

    public override string GetShown(PlayerScriptsHandler __playerScripts)
    {
        if(!_powerManager.IsMapPowered() && _needsPower) return "Power needs to be turned on!";
        if(__playerScripts.GetPlayerPerks().HasPerks(_perks) || __playerScripts.GetPlayerPerks().GetNumberOfPerks() >= 5 || (_buylimit != -1 && _bought > _buylimit)) return "";
        return $"E To Buy {_name}: <b>{_cost}</b> Points";
    }
}
