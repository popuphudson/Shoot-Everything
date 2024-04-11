using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MysteryBoxBuyable : Buyable
{
    [SerializeField] private Gun[] _allGuns;
    [SerializeField] private Transform _gunDisplay;
    [SerializeField] private Animator _anims;
    [SerializeField] private Vector3[] _locations;
    [SerializeField] private Vector3[] _rotations;
    private Gun _selectedGun;
    private float _grabTimer = 0;
    private bool _canGrab;
    private int _rollsUntilSwap;

    private void Start() {
        _rollsUntilSwap = Random.Range(5, 11);
        if(_locations.Length > 1) {
            int chosenLocation = Random.Range(0, _locations.Length);
            transform.localPosition = _locations[chosenLocation];
            transform.localEulerAngles = _rotations[chosenLocation];
        }
    }

    public void CanGrab() {
        _canGrab = true;
    }

    public void MoveLocation() {
        Vector3[] otherLocations = new Vector3[_locations.Length-1];
        Vector3[] otherRotations = new Vector3[_rotations.Length-1];
        int jindex = 0;
        for(int i = 0; i < _locations.Length; i++) {
            if(transform.position != _locations[i]) {
                otherLocations[jindex] = _locations[i];
                otherRotations[jindex] = _rotations[i];
                jindex++;
            }
        }
        int chosenLocation = Random.Range(0, otherLocations.Length);
        transform.localPosition = otherLocations[chosenLocation];
        transform.localEulerAngles = otherRotations[chosenLocation];
        return;
    }

    public override void Buy(PlayerScriptsHandler playerScripts)
    {
        if(_rollsUntilSwap <= 0 && _locations.Length > 1) {
            _anims.Play("Move");
        }
        if(_grabTimer > 0 && _canGrab) {
            playerScripts.GetPlayerGunInventory().AddGun(_selectedGun);
            _grabTimer = -1;
            _canGrab = false;
            for(int i = 0; i < _gunDisplay.childCount; i++) {
                Destroy(_gunDisplay.GetChild(i).gameObject);
            }
            return;
        }
        if(_grabTimer > 0) return;
        if(playerScripts.GetPlayerPoints().GetPoints() < _cost) return;
        playerScripts.GetPlayerPoints().RemovePoints(_cost);
        Gun[] gunPool = new Gun[_allGuns.Length-playerScripts.GetPlayerGunInventory().GetNumberOfGuns()];
        int index = 0;
        foreach(Gun gun in _allGuns) {
            if(!playerScripts.GetPlayerGunInventory().HasGun(gun)) {
                gunPool[index] = gun;
                index++;
            }
        }
        _selectedGun = gunPool[Random.Range(0, gunPool.Length)];
        _grabTimer = 10;
        for(int i = 0; i < _gunDisplay.childCount; i++) {
            Destroy(_gunDisplay.GetChild(i).gameObject);
        }
        _anims.Play("Random Selection");
        Instantiate(_selectedGun.GunModel, _gunDisplay);
        _rollsUntilSwap--;
    }

    public override string GetShown(PlayerScriptsHandler playerScripts)
    {
        if(_grabTimer > 0 && _canGrab) {
            return $"Grab {_selectedGun.name}";
        } else if(_grabTimer > 0){
            return "";
        }
        return $"Random Weapon: {_cost}";
    }

    private void Update() {
        if(_canGrab) _grabTimer -= Time.deltaTime;
        if(_grabTimer <= 0 && _canGrab) {
            _canGrab = false;
            for(int i = 0; i < _gunDisplay.childCount; i++) {
                Destroy(_gunDisplay.GetChild(i).gameObject);
            }
        }
    }
}
