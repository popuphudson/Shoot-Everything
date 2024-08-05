using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebrisBuyable : Interactable
{
    [SerializeField] private AudioManager _audioManager;
    [SerializeField] private int _cost;
    [SerializeField] private AreaData[] _areaDataLinksAffected;
    [SerializeField] private int[] _selectedAreaDataLinks;
    [SerializeField] private bool[] _areaDataLinkEnabled;   
    [SerializeField] private Sound _purchaseSound;
    private bool _removing = false;
    public override void Interact(PlayerScriptsHandler __playerScripts) {
        if(_removing) return;
        if(__playerScripts.GetPlayerPoints().GetPoints() < _cost) return;
        __playerScripts.GetPlayerPoints().RemovePoints(_cost);
        for(int i = 0; i < _areaDataLinksAffected.Length; i++) {
            _areaDataLinksAffected[i].AreaLinks[_selectedAreaDataLinks[i]].LinkEnabled = _areaDataLinkEnabled[i];
        }
        _audioManager.PlaySoundAtPoint(_purchaseSound, transform.position);
        StartCoroutine(Remove());
    }

    private IEnumerator Remove() {
        _removing = true;
        float init = transform.position.y;
        while(true) {
            transform.position -= new Vector3(0, Time.deltaTime*100, 0);
            yield return null;
            if(transform.position.y < init-10) {
                break;
            }
        }
        Destroy(gameObject);
    }

    public override string GetShown(PlayerScriptsHandler playerScripts, string __interactInput)
    {
        return $"{__interactInput} To Clear Debris: <b>{_cost}</b> Points";
    }
}
