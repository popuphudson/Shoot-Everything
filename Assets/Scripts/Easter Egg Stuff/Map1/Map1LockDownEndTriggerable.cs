using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map1LockDownEndTriggerable : Triggerable
{
    [SerializeField] private GameObject _door;
    [SerializeField] private ZombieSpawner _spawner; 
    public override void Trigger() {
        StartCoroutine(OpenDoor());
        _spawner.Pause();
    }

    IEnumerator OpenDoor() {
        _door.transform.Translate(0, -100, 0);
        yield return null;
        Destroy(_door);
    }
}
