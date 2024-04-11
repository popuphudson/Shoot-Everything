using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Buyable : MonoBehaviour
{
    [SerializeField] protected int _cost;
    public abstract void Buy(PlayerScriptsHandler playerScripts);
    public abstract string GetShown(PlayerScriptsHandler playerScripts);
}
