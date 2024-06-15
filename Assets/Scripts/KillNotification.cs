using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class KillNotification : MonoBehaviour
{
    [SerializeField] private GameObject _notification;
    private static GameObject _staticNotification;
    private static Transform _myself;
    private void Start() {
        _staticNotification = _notification;
        _myself = transform;
    }
    public static void Notification(string __notificationText) {
        GameObject Go = Instantiate(_staticNotification, _myself);
        Go.GetComponent<TextMeshProUGUI>().text = __notificationText;
        Go.GetComponent<UIMover>().MoveDir = 2*new Vector2(Random.Range(5,20), Random.Range(5, 20));
        Destroy(Go, 1f);
    }
}
