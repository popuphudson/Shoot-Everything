using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickNMixInputInteractable : MonoBehaviour, Interactable
{
    [SerializeField] private PickNMixBuyable _pnmBuyable;
    [SerializeField] private bool _isMainSide;
    [SerializeField] private Perks[] _selection;
    [SerializeField] private string[] _selectionNames;
    private int _selectedPerkIndex;
    public void Interact(PlayerScriptsHandler __playerScripts)
    {
        _selectedPerkIndex++;
        if(_selectedPerkIndex >= _selection.Length) {
            _selectedPerkIndex = 0;
        }
        _pnmBuyable.UpdateCanMix();
    }

    public string GetShown(PlayerScriptsHandler __playerScripts, string __interactInput)
    {
        if(_isMainSide) return $"{__interactInput} to Cycle main perk, Current perk: {_selectionNames[_selectedPerkIndex]}";
        return $"{__interactInput} to Cycle side perk, Current perk: {_selectionNames[_selectedPerkIndex]}";
    }

    public Perks GetSelectedPerk() {
        return _selection[_selectedPerkIndex];
    }
}
