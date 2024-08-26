using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Interactable
{
    public abstract void Interact(PlayerScriptsHandler __playerScripts);
    public abstract string GetShown(PlayerScriptsHandler __playerScripts, string __interactInput);
}
