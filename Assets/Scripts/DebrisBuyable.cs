using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebrisBuyable : Buyable
{
    [SerializeField] private AreaData[] _areaDataLinksAffected;
    [SerializeField] private int[] _selectedAreaDataLinks;
    [SerializeField] private bool[] _areaDataLinkEnabled;   
    private bool _removing = false;
    public override void Buy(PlayerScriptsHandler playerScripts) {
        if(_removing) return;
        if(playerScripts.GetPlayerPoints().GetPoints() < _cost) return;
        playerScripts.GetPlayerPoints().RemovePoints(_cost);
        for(int i = 0; i < _areaDataLinksAffected.Length; i++) {
            _areaDataLinksAffected[i].AreaLinks[_selectedAreaDataLinks[i]].LinkEnabled = _areaDataLinkEnabled[i];
        }
        StartCoroutine(Remove());
    }

    private IEnumerator Remove() {
        _removing = true;
        float init = transform.position.y;
        while(true) {
            transform.position -= new Vector3(0, Time.deltaTime*100, 0);
            yield return null;
            if(transform.position.y > init+100) {
                break;
            }
        }
        Destroy(gameObject);
    }

    public override string GetShown(PlayerScriptsHandler playerScripts)
    {
        return $"E To Clear Debris: <b>{_cost}</b> Points";
    }
}
