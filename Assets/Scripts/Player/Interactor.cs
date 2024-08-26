using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
public class Interactor : MonoBehaviour
{
    [SerializeField] private PlayerInput _playerInput;
    [SerializeField] private PlayerScriptsHandler _playerScripts;
    [SerializeField] private GameObject _costShower;
    [SerializeField] private TextMeshProUGUI _costText;
    private RectTransform _costShowerRectTransform;
    private RectTransform _costTextRectTransform;
    private bool _tryToBuy;
    private InputAction _interactInput;

    private void Start() {
        _costShowerRectTransform = _costShower.GetComponent<RectTransform>();
        _costTextRectTransform = _costText.GetComponent<RectTransform>();
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
            _costText.text = interactable.GetShown(_playerScripts, _interactInput.GetBindingDisplayString());
            _costShowerRectTransform.sizeDelta = new Vector2(_costTextRectTransform.sizeDelta.x+35, 100);
            _costShowerRectTransform.ForceUpdateRectTransforms();
            _costShower.SetActive(_costText.text!="");
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
