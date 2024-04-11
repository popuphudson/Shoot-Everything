using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebrisBuyable : Buyable
{
    private bool _removing = false;
    public override void Buy(PlayerScriptsHandler playerScripts) {
        if(_removing) return;
        if(playerScripts.GetPlayerPoints().GetPoints() < _cost) return;
        playerScripts.GetPlayerPoints().RemovePoints(_cost);
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
        return $"Debris {_cost}";
    }
}
