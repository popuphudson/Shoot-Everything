using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PackAPunchBuyable : Interactable
{
    [SerializeField] private AudioManager _audioManager;
    [SerializeField] private int _cost;
    [SerializeField] private float _packTime;
    [SerializeField] private Transform _shownGun;
    [SerializeField] private PauseMenu _pauseMenu;
    [SerializeField] private Sound _purchaseSound;
    private Gun _heldGun;
    private float _timer;
    public override void Interact(PlayerScriptsHandler __playerScripts)
    {
        if(_heldGun) {
            if(_timer > 0) return;
            __playerScripts.GetPlayerGunInventory().AddGun(_heldGun.PAPedWeapon);
            _heldGun = null;
            for(int i = 0; i < _shownGun.childCount; i++) {
                Destroy(_shownGun.GetChild(i).gameObject);
            }
            return;
        }
        if(__playerScripts.GetPlayerGunInventory().GetSelectedGun().IsPAPed) return;
        if(__playerScripts.GetPlayerPoints().GetPoints() < _cost) return;
        __playerScripts.GetPlayerPoints().RemovePoints(_cost);
        _audioManager.PlaySoundAtPoint(_purchaseSound, transform.position);
        _heldGun = __playerScripts.GetPlayerGunInventory().GetSelectedGun();
        __playerScripts.GetPlayerGunInventory().RemoveGun(_heldGun);
        _timer = _packTime;
        Instantiate(_heldGun.PAPedWeapon.GunModel, _shownGun);
        _shownGun.gameObject.SetActive(false);
    }

    public override string GetShown(PlayerScriptsHandler __playerScripts)
    {
        if(__playerScripts.GetPlayerGunInventory().GetSelectedGun() && __playerScripts.GetPlayerGunInventory().GetSelectedGun().IsPAPed) return "";
        if(_heldGun && _timer < 0) return "E to pickup weapon";
        if(!_heldGun) return $"E to Pack a Punch for {_cost}";
        return "Pack a punching";
    }

    private void Update() {
        if(_timer < 0 && _heldGun) _shownGun.gameObject.SetActive(true);
        if(_timer > -10 && !_pauseMenu.Paused) _timer -= Time.deltaTime;
        else if(_heldGun && _timer < -10) _heldGun = null;
    }
}
