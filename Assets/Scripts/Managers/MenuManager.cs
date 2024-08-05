using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.InputSystem;

public enum MenuState {
    MENU,
    SETTINGS,
    CONTROLS,
    MAPSELECT
}

public class MenuManager : MonoBehaviour
{
    [SerializeField] private PlayerInput _playerInput;
    [SerializeField] private EventSystem _eventSystem;
    [SerializeField] private GameObject _mapSelectMenu;
    [SerializeField] private GameObject _settingsMenu;
    [SerializeField] private GameObject _mainMenu;
    [SerializeField] private GameObject _controlsMenu;
    [SerializeField] private GameObject _startMain;
    [SerializeField] private GameObject _startSettings;
    [SerializeField] private GameObject _startSelect;
    private MenuState _menuState;
    private string _prevControlScheme;

    private void Start() {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
        _menuState = MenuState.MENU;
    }

    private void Update() {
        if(_prevControlScheme != _playerInput.currentControlScheme) {
            UpdateUI();
        }
        _prevControlScheme = _playerInput.currentControlScheme;
    }

    private void UpdateUI() {
        if(_playerInput.currentControlScheme == "Keyboard&Mouse") {
            _eventSystem.SetSelectedGameObject(null);
        } else {
            switch(_menuState) {
                case MenuState.MENU:
                    _eventSystem.SetSelectedGameObject(_startMain);
                    break;
                case MenuState.SETTINGS:
                    _eventSystem.SetSelectedGameObject(_startSettings);
                    break;
                case MenuState.MAPSELECT:
                    _eventSystem.SetSelectedGameObject(_startSelect);
                    break;
                case MenuState.CONTROLS:
                    break;
            }
        }
    }

    public void OpenSettings() {
        _settingsMenu.SetActive(true);
        _mapSelectMenu.SetActive(false);
        _mainMenu.SetActive(false);
        if(_playerInput.currentControlScheme != "Keyboard&Mouse") _eventSystem.SetSelectedGameObject(_startSettings);
        _menuState = MenuState.SETTINGS;
    }

    public void OpenMainMenu() {
        _settingsMenu.SetActive(false);
        _mapSelectMenu.SetActive(false);
        _mainMenu.SetActive(true);
        if(_playerInput.currentControlScheme != "Keyboard&Mouse") _eventSystem.SetSelectedGameObject(_startMain);
    }

    public void OpenMapSelect() {
        _settingsMenu.SetActive(false);
        _mapSelectMenu.SetActive(true);
        _mainMenu.SetActive(false);
        if(_playerInput.currentControlScheme != "Keyboard&Mouse") _eventSystem.SetSelectedGameObject(_startSelect);
    }

    public void OpenControls() {
        _controlsMenu.SetActive(true);
        _settingsMenu.SetActive(false);
    }

    public void CloseControls() {
        _controlsMenu.SetActive(false);
        _settingsMenu.SetActive(true);
    }

    public void Quit() {
        Application.Quit();
    }
}
