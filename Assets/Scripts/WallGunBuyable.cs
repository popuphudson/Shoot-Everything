using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallGunBuyable : Interactable
{
    [SerializeField] private int _cost;
    [SerializeField] private string _gunName;
    [SerializeField] private int _ammoCost;
    [SerializeField] private Gun _gunGiven;
    public override void Interact(PlayerScriptsHandler __playerScripts)
    {
        if(__playerScripts.GetPlayerGunInventory().HasGun(_gunGiven)) {
            if(__playerScripts.GetPlayerPoints().GetPoints() < _ammoCost) return;
            if(__playerScripts.GetPlayerGunInventory().GunAtFullAmmo(_gunGiven)) return;
            __playerScripts.GetPlayerPoints().RemovePoints(_ammoCost);
            __playerScripts.GetPlayerGunInventory().AddAmmo(_gunGiven);
        } else {
            if(__playerScripts.GetPlayerPoints().GetPoints() < _cost) return;
            __playerScripts.GetPlayerPoints().RemovePoints(_cost);
            __playerScripts.GetPlayerGunInventory().AddGun(_gunGiven);
        }
    }

    public override string GetShown(PlayerScriptsHandler __playerScripts)
    {
        if(__playerScripts.GetPlayerGunInventory().HasGun(_gunGiven)) {
            return $"E To Buy {_gunName} Ammo: <b>{_ammoCost}</b> Points"; 
        }
        return $"E To Buy {_gunName}: <b>{_cost}</b> Points";
    }
}
