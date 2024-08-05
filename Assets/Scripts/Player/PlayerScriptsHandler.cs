using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScriptsHandler : MonoBehaviour
{
    [SerializeField] private PlayerPoints _points;
    [SerializeField] private PlayerHealth _health;
    [SerializeField] private PlayerPerks _perks;
    [SerializeField] private PlayerMovement _movement;
    [SerializeField] private GunHandler _gunHandler;
    [SerializeField] private PlayerInventory _playerInventory;
    [SerializeField] private PlayerLook _playerLook;

    public PlayerLook GetPlayerLook() {
        return _playerLook;
    }

    public PlayerPoints GetPlayerPoints() {
        return _points;
    }

    public PlayerHealth GetPlayerHealth() {
        return _health;
    }

    public PlayerPerks GetPlayerPerks() {
        return _perks;
    }

    public PlayerMovement GetPlayerMovement() {
        return _movement;
    }

    public GunHandler GetPlayerGunHandler() {
        return _gunHandler;
    }

    public PlayerInventory GetPlayerInventory() {
        return _playerInventory;
    }
}
