using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.UI;
public class Interactor : MonoBehaviour
{
    [SerializeField] private PlayerInput _playerInput;
    [SerializeField] private PlayerScriptsHandler _playerScripts;
    [SerializeField] private GameObject _costShower;
    [SerializeField] private TextMeshProUGUI _costTextSize;
    [SerializeField] private TextMeshProUGUI _costTextShown;
    private bool _tryToBuy;
    private InputAction _interactInput;

    private void Start() {
        _interactInput = _playerInput.actions["Interact"];
    }

    private void Update() {
        if(_interactInput.WasPressedThisFrame()) {
            _tryToBuy = true;
        } else if(_interactInput.WasReleasedThisFrame()) {
            _tryToBuy = false;
        }
    }

    private void OnTriggerStay(Collider __other) {
        if(__other.CompareTag("Buyable")) {
            Interactable interactable = __other.GetComponent<Interactable>();
            _costTextSize.text = interactable.GetShown(_playerScripts, _interactInput.GetBindingDisplayString());
            _costTextShown.text = _costTextSize.text;
            _costShower.SetActive(_costTextShown.text!="");
            if(_tryToBuy) {
                interactable.Interact(_playerScripts);
                _tryToBuy = false;
            }
        }
    }

    private void OnTriggerExit(Collider __other) {
        if(__other.CompareTag("Buyable")) {
            _costShower.SetActive(false);
        }
    }
}
