using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SeriesSection {
    public string RequirementMessage;
    public string SuccessMessage;
    public string[] ItemsRequired;
    public string[] ItemsGiven;
}

public class SeriesInteractable : MonoBehaviour, Interactable 
{
    [SerializeField] private PowerManager _powerManager;
    [SerializeField] private bool _needsPower;
    [SerializeField] private SeriesSection[] _sections;
    private int _sectionIndex = 0;

    public void Interact(PlayerScriptsHandler __playerScripts) {
        if(_sectionIndex >= _sections.Length) return;
        if(!__playerScripts.GetPlayerInventory().HasItems(_sections[_sectionIndex].ItemsRequired)) return; 
        foreach(string item in _sections[_sectionIndex].ItemsGiven) {
            __playerScripts.GetPlayerInventory().AddItem(item);
        }
        _sectionIndex++;
    }

    public string GetShown(PlayerScriptsHandler __playerScripts, string __interactInput) {
        if(_sectionIndex >= _sections.Length) return "";
        if(_needsPower && !_powerManager.IsMapPowered()) return "Needs Power...";
        if(!__playerScripts.GetPlayerInventory().HasItems(_sections[_sectionIndex].ItemsRequired)) return _sections[_sectionIndex].RequirementMessage;
        return $"{__interactInput} to {_sections[_sectionIndex].SuccessMessage}";
    }
}
