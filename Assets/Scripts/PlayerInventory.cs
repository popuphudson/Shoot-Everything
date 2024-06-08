using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    private List<string> _items = new List<string>();

    public void AddItem(string __item) {
        _items.Add(__item);
    }

    public bool HasItem(string __item) {
        return _items.Contains(__item);
    }

    public void RemoveItem(string __item) {
        _items.Remove(__item);
    }
}
