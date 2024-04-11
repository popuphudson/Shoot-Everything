using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class DeathScreen : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _gameData;
    [SerializeField] private PowerUpManager _powerUpManager;
    [SerializeField] private ZombieSpawner _zombieSpawner;
    [SerializeField] private PlayerPoints _playerPoints;
    public void BackToMenu() {
        SceneManager.LoadSceneAsync(0);
    }

    public void Died() {
        _gameData.text = $"<b>Round Survived: {_zombieSpawner.GetRound()}</b>\nKills: {_powerUpManager.GetTotalKills()}\nTotal Points: {_playerPoints.GetTotalPoints()}";
    }
}
