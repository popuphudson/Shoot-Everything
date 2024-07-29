using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private GameObject _mapSelectMenu;
    [SerializeField] private GameObject _settingsMenu;
    [SerializeField] private GameObject _mainMenu;

    private void Start() {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
    }

    public void OpenSettings() {
        _settingsMenu.SetActive(true);
        _mapSelectMenu.SetActive(false);
        _mainMenu.SetActive(false);
    }

    public void OpenMainMenu() {
        _settingsMenu.SetActive(false);
        _mapSelectMenu.SetActive(false);
        _mainMenu.SetActive(true);
    }

    public void OpenMapSelect() {
        _settingsMenu.SetActive(false);
        _mapSelectMenu.SetActive(true);
        _mainMenu.SetActive(false);
    }

    public void Quit() {
        Application.Quit();
    }
}
