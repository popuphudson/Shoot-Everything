using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapSelectionMenu : MonoBehaviour
{
    [SerializeField] private GameObject _startButton;
    [SerializeField] private GameObject _background;
    [SerializeField] private GameObject[] _mapDescriptions;
    [SerializeField] private Animator _screenFade;
    private int _selectedMap = 0;

    public void ShowMapData(int __mapIndex) {
        _startButton.SetActive(true);
        _background.SetActive(true);
        foreach(GameObject desc in _mapDescriptions) {
            desc.SetActive(false);
        }
        _mapDescriptions[__mapIndex].SetActive(true);
        _selectedMap = __mapIndex+1;
    }

    public void StartGame() {
        _screenFade.Play("Fade Out");
        StartCoroutine(LoadGameScene());
    }

    IEnumerator LoadGameScene() {
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene(_selectedMap);
    }

    public void Back() {
        _startButton.SetActive(false);
        _background.SetActive(false);
        foreach(GameObject desc in _mapDescriptions) {
            desc.SetActive(false);
        }
    }
}
