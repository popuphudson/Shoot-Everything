using System.Collections;
using UnityEngine;

public class OpenableDoor : MonoBehaviour {
    public void OpenDoor() {
        StartCoroutine(DoorOpening());
    }

    IEnumerator DoorOpening() {
        transform.Translate(0, -100, 0);
        yield return null;
        Destroy(gameObject);
    }
}
