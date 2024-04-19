using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallGunBuyable : Buyable
{
    [SerializeField] private string _gunName;
    [SerializeField] private int _ammoCost;
    [SerializeField] private Gun _gunGiven;
    public override void Buy(PlayerScriptsHandler playerScripts)
    {
        if(playerScripts.GetPlayerGunInventory().HasGun(_gunGiven)) {
            if(playerScripts.GetPlayerPoints().GetPoints() < _ammoCost) return;
            if(playerScripts.GetPlayerGunInventory().GunAtFullAmmo(_gunGiven)) return;
            playerScripts.GetPlayerPoints().RemovePoints(_ammoCost);
            playerScripts.GetPlayerGunInventory().AddAmmo(_gunGiven);
        } else {
            if(playerScripts.GetPlayerPoints().GetPoints() < _cost) return;
            playerScripts.GetPlayerPoints().RemovePoints(_cost);
            playerScripts.GetPlayerGunInventory().AddGun(_gunGiven);
        }
    }

    public override string GetShown(PlayerScriptsHandler _playerScripts)
    {
        if(_playerScripts.GetPlayerGunInventory().HasGun(_gunGiven)) {
            return $"E To Buy {_gunName} Ammo: <b>{_ammoCost}</b> Points"; 
        }
        return $"E To Buy {_gunName}: <b>{_cost}</b> Points";
    }
}
