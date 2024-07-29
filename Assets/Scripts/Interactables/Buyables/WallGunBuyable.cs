using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder.MeshOperations;

public class WallGunBuyable : Interactable
{
    [SerializeField] private AudioManager _audioManager;
    [SerializeField] private int _cost;
    [SerializeField] private int _ammoCost;
    [SerializeField] private int _packedAmmoCost;
    [SerializeField] private Gun _gunGiven;
    [SerializeField] private Gun _packedGun;
    [SerializeField] private Sound _purchaseSound;
    public override void Interact(PlayerScriptsHandler __playerScripts)
    {
        if(__playerScripts.GetPlayerGunInventory().HasGun(_packedGun)) {

            if(__playerScripts.GetPlayerPoints().GetPoints() < _packedAmmoCost) return;
            if(__playerScripts.GetPlayerGunInventory().GunAtFullAmmo(_packedGun)) return;

            __playerScripts.GetPlayerPoints().RemovePoints(_packedAmmoCost);
            __playerScripts.GetPlayerGunInventory().AddAmmo(_packedGun);

        } else if(__playerScripts.GetPlayerGunInventory().HasGun(_gunGiven)) {

            if(__playerScripts.GetPlayerPoints().GetPoints() < _ammoCost) return;
            if(__playerScripts.GetPlayerGunInventory().GunAtFullAmmo(_gunGiven)) return;

            __playerScripts.GetPlayerPoints().RemovePoints(_ammoCost);
            __playerScripts.GetPlayerGunInventory().AddAmmo(_gunGiven);

        } else {

            if(__playerScripts.GetPlayerPoints().GetPoints() < _cost) return;

            __playerScripts.GetPlayerPoints().RemovePoints(_cost);
            __playerScripts.GetPlayerGunInventory().AddGun(_gunGiven);

        }

        _audioManager.PlaySoundAtPoint(_purchaseSound, transform.position);
    }

    public override string GetShown(PlayerScriptsHandler __playerScripts)
    {
        if(__playerScripts.GetPlayerGunInventory().HasGun(_packedGun)) return $"E To Buy {_packedGun.Name} Ammo: <b>{_packedAmmoCost}</b> Points"; 
        if(__playerScripts.GetPlayerGunInventory().HasGun(_gunGiven)) return $"E To Buy {_gunGiven.Name} Ammo: <b>{_ammoCost}</b> Points"; 
        return $"E To Buy {_gunGiven.Name}: <b>{_cost}</b> Points";
    }
}
