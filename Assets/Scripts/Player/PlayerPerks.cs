using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Flags]
public enum Perks {
    NONE = 0,
    EXTRA_HEALTH = 1,
    EXTRA_OVERALL_DAMAGE = 1 << 1,
    QUICK_HEAL_LIFE = 1 << 2,
    FAST_RELOAD = 1 << 3,
    BETTER_RUN = 1 << 4
}

public class PlayerPerks : MonoBehaviour
{
    [SerializeField] private Transform _perksUI;
    [SerializeField] private GameObject[] _perkPrefabs;
    private List<int> _perkOrder = new List<int>();
    private Perks _perks;
    private Perks _mixMainPerk, _mixSidePerk;

    public int GetNumberOfPerks() {
        return _perkOrder.Count;
    }

    public void AddPerks(Perks __perks) {
        _perks = _perks | __perks;
        if((Perks.EXTRA_HEALTH & __perks) != 0) {
            Instantiate(_perkPrefabs[0], _perksUI);
            _perkOrder.Add(0);
        } 
        if((Perks.EXTRA_OVERALL_DAMAGE & __perks) != 0) {
            Instantiate(_perkPrefabs[1], _perksUI);
            _perkOrder.Add(1);
        } 
        if((Perks.QUICK_HEAL_LIFE & __perks) != 0) {
            Instantiate(_perkPrefabs[2], _perksUI);
            _perkOrder.Add(2);
        } 
        if((Perks.FAST_RELOAD & __perks) != 0) {
            Instantiate(_perkPrefabs[3], _perksUI);
            _perkOrder.Add(3);
        } 
        if((Perks.BETTER_RUN & __perks) != 0) {
            Instantiate(_perkPrefabs[4], _perksUI);
            _perkOrder.Add(4);
        } 
    }
    
    public void RemovePerks(Perks __perks) {
        _perks = _perks & ~__perks;
        if((Perks.EXTRA_HEALTH & __perks) != 0) {
            Destroy(_perksUI.GetChild(_perkOrder.IndexOf(0)).gameObject);
        }  
        if((Perks.EXTRA_OVERALL_DAMAGE & __perks) != 0) {
            Destroy(_perksUI.GetChild(_perkOrder.IndexOf(1)).gameObject);
        } 
        if((Perks.QUICK_HEAL_LIFE & __perks) != 0) {
            Debug.Log(_perkOrder.IndexOf(2));
            Destroy(_perksUI.GetChild(_perkOrder.IndexOf(2)).gameObject);
        }
        if((Perks.FAST_RELOAD & __perks) != 0) {
            Destroy(_perksUI.GetChild(_perkOrder.IndexOf(3)).gameObject);
        } 
        if((Perks.BETTER_RUN & __perks) != 0) {
            Destroy(_perksUI.GetChild(_perkOrder.IndexOf(4)).gameObject);
        }
        
        if((Perks.EXTRA_HEALTH & __perks) != 0) {
            _perkOrder.Remove(0);
        }  
        if((Perks.EXTRA_OVERALL_DAMAGE & __perks) != 0) {
            _perkOrder.Remove(1);
        } 
        if((Perks.QUICK_HEAL_LIFE & __perks) != 0) {
            Debug.Log(_perkOrder.IndexOf(2));
            _perkOrder.Remove(2);
        }
        if((Perks.FAST_RELOAD & __perks) != 0) {
            _perkOrder.Remove(3);
        } 
        if((Perks.BETTER_RUN & __perks) != 0) {
            _perkOrder.Remove(4);
        }
    }

    public void RemovePerk(Perks __perk) {
        _perks = _perks & ~__perk;
        if((Perks.EXTRA_HEALTH & __perk) != 0) {
            Destroy(_perksUI.GetChild(_perkOrder.IndexOf(0)).gameObject);
            _perkOrder.Remove(0);
        } else if((Perks.EXTRA_OVERALL_DAMAGE & __perk) != 0) {
            Destroy(_perksUI.GetChild(_perkOrder.IndexOf(1)).gameObject);
            _perkOrder.Remove(1);
        } else if((Perks.QUICK_HEAL_LIFE & __perk) != 0) {
            Debug.Log(_perkOrder.IndexOf(2));
            Destroy(_perksUI.GetChild(_perkOrder.IndexOf(2)).gameObject);
            _perkOrder.Remove(2);
        } else if((Perks.FAST_RELOAD & __perk) != 0) {
            Destroy(_perksUI.GetChild(_perkOrder.IndexOf(3)).gameObject);
            _perkOrder.Remove(3);
        } else if((Perks.BETTER_RUN & __perk) != 0) {
            Destroy(_perksUI.GetChild(_perkOrder.IndexOf(4)).gameObject);
            _perkOrder.Remove(4);
        }
    }

    public void SetMixPerks(Perks __mainPerk, Perks __sidePerk) {
        _mixMainPerk = __mainPerk;
        _mixSidePerk = __sidePerk;
        int mainPerkIndex = Mathf.FloorToInt(Mathf.Log((float)__mainPerk, 2f));
        int sidePerkIndex = Mathf.FloorToInt(Mathf.Log((float)__sidePerk, 2f));
        RectTransform mixShown = Instantiate(_perkPrefabs[sidePerkIndex], _perksUI).GetComponent<RectTransform>();
        RectTransform mainPart = Instantiate(_perkPrefabs[mainPerkIndex], mixShown.transform).GetComponent<RectTransform>();
        mainPart.localScale = new Vector3(0.75f, 0.75f, 0.75f);
    }

    public bool HasMix(Perks __mainPerk, Perks __sidePerk) {
        return HasMainMixPerk(__mainPerk) && HasSideMixPerk(__sidePerk);
    }

    public bool HasPerks(Perks __perks) {
        return (_perks & __perks) != 0;
    }

    public bool HasMainMixPerk(Perks __perk) {
        return (_mixMainPerk & __perk) != 0;
    }

    public bool HasSideMixPerk(Perks __perk) {
        return (_mixSidePerk & __perk) != 0;
    }
}
