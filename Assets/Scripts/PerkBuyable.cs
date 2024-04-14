using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerkBuyable : Buyable
{
    [SerializeField] private string _name;
    [SerializeField] private Perks _perks;
    [SerializeField] private int _buylimit;
    [SerializeField] private MeshRenderer _render;
    private int _bought = 0;

    private void Start() {
        _bought = 0;
    }

    public override void Buy(PlayerScriptsHandler playerScripts)
    {
        if(_buylimit != -1 && _bought > _buylimit) return;
        if(playerScripts.GetPlayerPerks().GetNumberOfPerks() >= 4) return;
        if(playerScripts.GetPlayerPoints().GetPoints() < _cost) return;
        PlayerPerks playerPerks = playerScripts.GetPlayerPerks();
        if(playerPerks.HasPerks(_perks)) return;
        playerScripts.GetPlayerPoints().RemovePoints(_cost);
        playerPerks.AddPerks(_perks);
        _bought++;
        if(_bought > _buylimit) {
            _render.enabled = false;
        }
    }

    public override string GetShown(PlayerScriptsHandler playerScripts)
    {
        if(playerScripts.GetPlayerPerks().HasPerks(_perks) || playerScripts.GetPlayerPerks().GetNumberOfPerks() >= 4 || (_buylimit != -1 && _bought > _buylimit)) {
            return "";
        } else {
            return $"{_name}: {_cost}";
        }
    }
}
