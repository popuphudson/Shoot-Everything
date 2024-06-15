using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class Interactor : MonoBehaviour
{
    [SerializeField] private PlayerScriptsHandler _playerScripts;
    [SerializeField] private GameObject _costShower;
    [SerializeField] private TextMeshProUGUI _costText;
    private RectTransform _costShowerRectTransform;
    private RectTransform _costTextRectTransform;
    private bool _tryToBuy;

    private void Start() {
        _costShowerRectTransform = _costShower.GetComponent<RectTransform>();
        _costTextRectTransform = _costText.GetComponent<RectTransform>();
    }

    private void Update() {
        if(Input.GetKeyDown(KeyCode.E)) {
            _tryToBuy = true;
        } else if(Input.GetKeyUp(KeyCode.E)) {
            _tryToBuy = false;
        }
    }

    private void OnTriggerStay(Collider other) {
        if(other.CompareTag("Buyable")) {
            Interactable interactable = other.GetComponent<Interactable>();
            _costText.text = interactable.GetShown(_playerScripts);
            _costShowerRectTransform.sizeDelta = new Vector2(_costTextRectTransform.sizeDelta.x+35, 100);
            _costShower.SetActive(_costText.text!="");
            if(_tryToBuy) {
                interactable.Interact(_playerScripts);
                _tryToBuy = false;
            }
        }
    }

    private void OnTriggerExit(Collider other) {
        if(other.CompareTag("Buyable")) {
            _costShower.SetActive(false);
        }
    }
}
