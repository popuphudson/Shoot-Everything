using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public enum PauseMenuState {
    MENU,
    CONFIRMATION,
    SETTINGS,
    CONTROLS
}

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private EventSystem _eventSystem;
    [SerializeField] private PlayerInput _playerInput;
    [SerializeField] private PlayerLook _playerLook;
    [SerializeField] private GameObject _pauseMenu;
    [SerializeField] private GameObject _pauseMain;
    [SerializeField] private GameObject _pauseSettings; 
    [SerializeField] private GameObject _pauseControls;
    [SerializeField] private GameObject _pauseConfirmation;
    [SerializeField] private GameObject _startMenu;
    [SerializeField] private GameObject _startConfirm;
    [SerializeField] private GameObject _startSettings;
    public bool Paused = false;
    private InputAction _pauseInput;
    private PauseMenuState _menuState;
    private string _prevControlScheme;

    private void Start() {
        _pauseInput = _playerInput.actions["TogglePause"];
        _menuState = PauseMenuState.MENU;
    }

    private void Update() {
        if(_pauseInput.WasPressedThisFrame()) {
            Paused = !Paused;
            if(Paused) {
                Pause();
            } else {
                Resume();
            }
        }
        if(Paused) {
            if(_playerInput.currentControlScheme != _prevControlScheme) {
                UpdateUI();
            }
            _prevControlScheme = _playerInput.currentControlScheme;
        }
    }

    private void UpdateUI() {
        if(_playerInput.currentControlScheme == "Keyboard And Mouse") {
            _eventSystem.SetSelectedGameObject(null);
        } else {
            switch(_menuState) {
                case PauseMenuState.MENU:
                    _eventSystem.SetSelectedGameObject(_startMenu);
                    break;
                case PauseMenuState.SETTINGS:
                    _eventSystem.SetSelectedGameObject(_startSettings);
                    break;
                case PauseMenuState.CONFIRMATION:
                    _eventSystem.SetSelectedGameObject(_startConfirm);
                    break;
                case PauseMenuState.CONTROLS:
                    break;
            }
        }
    }

    private void Pause() {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
        Time.timeScale = 0;
        _pauseMenu.SetActive(true);
        _pauseControls.SetActive(false);
        OpenPauseMainMenu();
        if(_playerInput.currentControlScheme != "Keyboard And Mouse") _eventSystem.SetSelectedGameObject(_startMenu);
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

    public void OpenSettings() {
        _pauseSettings.SetActive(true);
        _pauseMain.SetActive(false);
        if(_playerInput.currentControlScheme != "Keyboard And Mouse") _eventSystem.SetSelectedGameObject(_startSettings);
    }

    public void OpenPauseMainMenu() {
        _pauseSettings.SetActive(false);
        _pauseMain.SetActive(true);
        if(_playerInput.currentControlScheme != "Keyboard And Mouse") _eventSystem.SetSelectedGameObject(_startMenu);
    }

    public void AskBackToMenu() {
        _pauseConfirmation.SetActive(true);
        if(_playerInput.currentControlScheme != "Keyboard And Mouse") _eventSystem.SetSelectedGameObject(_startConfirm);
    }

    public void DeclineBackToMenu() {
        _pauseConfirmation.SetActive(false);
        if(_playerInput.currentControlScheme != "Keyboard And Mouse") _eventSystem.SetSelectedGameObject(_startMenu);
    }

    public void BackToGame() {
        Paused = false;
        Resume();
    }

    public void BackToMenu() {
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }

    public void OpenControls() {
        _pauseSettings.SetActive(false);
        _pauseControls.SetActive(true);
    }

    public void CloseControls() {
        _pauseSettings.SetActive(true);
        _pauseControls.SetActive(false);
    }
}
