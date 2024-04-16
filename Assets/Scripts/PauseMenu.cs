using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private PlayerLook _playerLook;
    [SerializeField] private GameObject _pauseMenu;
    [SerializeField] private GameObject _pauseMain;
    [SerializeField] private GameObject _pauseSettings; 
    [SerializeField] private GameObject _pauseConfirmation;
    public bool Paused = false;
    private void Update() {
        if(Input.GetKeyDown(KeyCode.Escape)) {
            Paused = !Paused;
            if(Paused) {
                Pause();
            } else {
                Resume();
            }
        }
    }

    private void Pause() {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
        Time.timeScale = 0;
        _pauseMenu.SetActive(true);
    }

    public void UpdatedFOV() {
        _playerLook.UpdateFOV();
    }

    public void UpdatedMouseSense() {
        _playerLook.UpdateSense();
    }

    private void Resume() {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Time.timeScale = 1;
        _pauseMenu.SetActive(false);
        UpdatedFOV();
        UpdatedMouseSense();
    }

    public void OpenSettigns() {
        _pauseSettings.SetActive(true);
        _pauseMain.SetActive(false);
    }

    public void OpenPauseMainMenu() {
        _pauseSettings.SetActive(false);
        _pauseMain.SetActive(true);
    }

    public void AskBackToMenu() {
        _pauseConfirmation.SetActive(true);
    }

    public void DeclineBackToMenu() {
        _pauseConfirmation.SetActive(false);
    }

    public void BackToGame() {
        Paused = false;
        Resume();
    }

    public void BackToMenu() {
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }
}
