using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerSwitchBuyable : Buyable
{
    [SerializeField] private ParticleSystem _sparks;
    [SerializeField] private Transform _powerSwitch;
    [SerializeField] private PowerManager _powerManager;
    public override void Buy(PlayerScriptsHandler playerScripts)
    {
        if(_powerManager.IsMapPowered()) return;
        _powerManager.PowerMap();
        _powerSwitch.rotation = Quaternion.Euler(70, 0, 0);
        _sparks.Play();
    }

    public override string GetShown(PlayerScriptsHandler playerScripts)
    {
        if(_powerManager.IsMapPowered()) return "";
        return "E To Turn On Power!";
    }
}
