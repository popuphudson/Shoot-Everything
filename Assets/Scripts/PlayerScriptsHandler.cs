using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScriptsHandler : MonoBehaviour
{
    [SerializeField] private PlayerPoints _points;
    [SerializeField] private PlayerHealth _health;
    [SerializeField] private PlayerPerks _perks;
    [SerializeField] private PlayerMovement _movement;
    [SerializeField] private GunInventory _gunInventory;

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

    public GunInventory GetPlayerGunInventory() {
        return _gunInventory;
    }
}
