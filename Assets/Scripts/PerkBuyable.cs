using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerkBuyable : Buyable
{
    [SerializeField] private string _name;
    [SerializeField] private Perks _perks;
    public override void Buy(PlayerScriptsHandler playerScripts)
    {
        if(playerScripts.GetPlayerPerks().GetNumberOfPerks() >= 4) return;
        if(playerScripts.GetPlayerPoints().GetPoints() < _cost) return;
        PlayerPerks playerPerks = playerScripts.GetPlayerPerks();
        if(playerPerks.HasPerks(_perks)) return;
        playerScripts.GetPlayerPoints().RemovePoints(_cost);
        playerPerks.AddPerks(_perks);
    }

    public override string GetShown(PlayerScriptsHandler playerScripts)
    {
        if(playerScripts.GetPlayerPerks().HasPerks(_perks) || playerScripts.GetPlayerPerks().GetNumberOfPerks() >= 4) {
            return "";
        } else {
            return $"{_name}: {_cost}";
        }
    }
}
