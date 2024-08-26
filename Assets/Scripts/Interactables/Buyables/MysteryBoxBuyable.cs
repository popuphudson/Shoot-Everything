using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MysteryBoxBuyable : MonoBehaviour, Interactable
{
    [SerializeField] private AudioManager _audioManager;
    [SerializeField] private int _cost;
    [SerializeField] private Gun[] _allGuns;
    [SerializeField] private Gun _starterPistol;
    [SerializeField] private Transform _gunDisplay;
    [SerializeField] private Animator _anims;
    [SerializeField] private Vector3[] _locations;
    [SerializeField] private Vector3[] _rotations;
    [SerializeField] private PauseMenu _pauseMenu;
    [SerializeField] private Sound _purchaseSound;
    private Gun _selectedGun;
    private float _grabTimer = 0;
    private bool _canGrab;
    private int _rollsUntilSwap;
    private bool _moving;

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
        Vector3[] otherLocations = new Vector3[_locations.Length];
        Vector3[] otherRotations = new Vector3[_rotations.Length];
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

    public void StopMoving() {
        _moving = false;
    }

    public void Interact(PlayerScriptsHandler __playerScripts)
    {
        if(_grabTimer > 0 && _canGrab) {
            __playerScripts.GetPlayerGunHandler().AddGun(_selectedGun);
            _grabTimer = -1;
            _canGrab = false;
            for(int i = 0; i < _gunDisplay.childCount; i++) {
                Destroy(_gunDisplay.GetChild(i).gameObject);
            }
            return;
        }
        if(_grabTimer > 0) return;
        if(_rollsUntilSwap <= 0 && _locations.Length > 1) {
            _rollsUntilSwap = Random.Range(5, 11);
            _anims.Play("Move");
            _moving = true;
            return;
        }
        if(_moving) return;
        if(__playerScripts.GetPlayerPoints().GetPoints() < _cost) return;
        __playerScripts.GetPlayerPoints().RemovePoints(_cost);
        _audioManager.PlaySoundAtPoint(_purchaseSound, transform.position);
        List<Gun> gunPool = new List<Gun>();
        foreach(Gun gun in _allGuns) {
            if(!__playerScripts.GetPlayerGunHandler().HasGun(gun) && !__playerScripts.GetPlayerGunHandler().HasGun(gun.PAPedWeapon)) {
                gunPool.Add(gun);
            }
        }
        _selectedGun = gunPool[Random.Range(0, gunPool.Count)];
        _grabTimer = 10;
        for(int i = 0; i < _gunDisplay.childCount; i++) {
            Destroy(_gunDisplay.GetChild(i).gameObject);
        }
        _anims.Play("Random Selection");
        Instantiate(_selectedGun.GunModel, _gunDisplay);
        _rollsUntilSwap--;
    }

    public string GetShown(PlayerScriptsHandler __playerScripts, string __interactInput)
    {
        if(_grabTimer > 0 && _canGrab && _selectedGun) {
            return $"E To Grab {_selectedGun.name}";
        } else if(_grabTimer > 0){
            return "";
        }
        return $"{__interactInput} To Buy Random Weapon: <b>{_cost}</b> Points";
    }

    private void Update() {
        if(_canGrab && !_pauseMenu.Paused) _grabTimer -= Time.deltaTime;
        if(_grabTimer <= 0 && _canGrab) {
            _canGrab = false;
            for(int i = 0; i < _gunDisplay.childCount; i++) {
                Destroy(_gunDisplay.GetChild(i).gameObject);
            }
        }
    }
}
