using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Mixable {
    public Perks MainPerk;
    public Perks SidePerk;
}

public class PickNMixBuyable : Interactable
{
    [SerializeField] private AudioManager _audioManager;
    [SerializeField] private int _cost;
    [SerializeField] private PickNMixInputInteractable _mainInput, _sideInput;
    [SerializeField] private Mixable[] _mixablePerks;
    [SerializeField] private Sound _purchaseSound;
    private bool _mixed = false;
    private bool _canMix = false;

    private void Start() {
        UpdateCanMix();
    }

    public override void Interact(PlayerScriptsHandler __playerScripts)
    {
        if(_mainInput.GetSelectedPerk() == _sideInput.GetSelectedPerk()) return;
        if(!_canMix) return;
        if(!__playerScripts.GetPlayerPerks().HasPerks(_mainInput.GetSelectedPerk()) || !__playerScripts.GetPlayerPerks().HasPerks(_sideInput.GetSelectedPerk())) return;
        if(__playerScripts.GetPlayerPoints().GetPoints() < _cost) return;
        __playerScripts.GetPlayerPoints().RemovePoints(_cost);
        _audioManager.PlaySoundAtPoint(_purchaseSound, transform.position);
        __playerScripts.GetPlayerPerks().RemovePerks(_mainInput.GetSelectedPerk()|_sideInput.GetSelectedPerk());
        __playerScripts.GetPlayerPerks().SetMixPerks(_mainInput.GetSelectedPerk(), _sideInput.GetSelectedPerk());
        _mixed = true;
    }

    public override string GetShown(PlayerScriptsHandler __playerScripts)
    {
        if(_mixed) return "";
        if(_mainInput.GetSelectedPerk() == _sideInput.GetSelectedPerk()) return "Cannot mix the same perk together!";
        if(!_canMix) return "A strange force prevents you from mixing these two perks";
        if(!__playerScripts.GetPlayerPerks().HasPerks(_mainInput.GetSelectedPerk()) || !__playerScripts.GetPlayerPerks().HasPerks(_sideInput.GetSelectedPerk()))  return "You need the selected perks";
        return $"E to mix perks (Only once) {_cost} Points";
    }

    public void UpdateCanMix() {
        _canMix = false;
        foreach(Mixable mixable in _mixablePerks) {
            if(mixable.MainPerk == _mainInput.GetSelectedPerk() && mixable.SidePerk == _sideInput.GetSelectedPerk()) {
                _canMix = true;
                return;
            }
        }
    }
}
