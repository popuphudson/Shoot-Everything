using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    public abstract void Interact(PlayerScriptsHandler __playerScripts);
    public abstract string GetShown(PlayerScriptsHandler __playerScripts);
}
