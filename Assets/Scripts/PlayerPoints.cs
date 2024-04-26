using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerPoints : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _pointsText;
    [SerializeField] private int _startingPoints = 500;
    [SerializeField] private RectTransform _notification;
    [SerializeField] private PowerUpManager _powerUpManager;
    private int _points;
    private int _totalPoints;

    private void Start() {
        #if UNITY_EDITOR
            AddPoints(100000000);
        #else
            AddPoints(_startingPoints);    
        #endif
    }

    public int GetPoints() {
        return _points;
    }    

    public int GetTotalPoints() {
        return _totalPoints;
    }

    public void AddPoints(int amount) {
        _points += amount*(_powerUpManager.IsPowerupActive(PowerupType.DOUBLE_POINTS)?2:1);
        _totalPoints += amount*(_powerUpManager.IsPowerupActive(PowerupType.DOUBLE_POINTS)?2:1);
        _pointsText.text = $"Points: {_points}";
        _notification.localPosition = new Vector3(-755+((_pointsText.text.Length-9)*30), -475, 0);
        KillNotification.Notification($"{amount*(_powerUpManager.IsPowerupActive(PowerupType.DOUBLE_POINTS)?2:1)}");
    }

    public void RemovePoints(int amount) {
        _points -= amount;
        _points = Mathf.Max(_points, 0);
        _pointsText.text = $"Points: {_points}";
        _notification.localPosition = new Vector3(-755+((_pointsText.text.Length-9)*30), -475, 0);
    }
}
