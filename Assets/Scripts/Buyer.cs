using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class Buyer : MonoBehaviour
{
    [SerializeField] private PlayerScriptsHandler _playerScripts;
    [SerializeField] private GameObject _costShower;
    [SerializeField] private TextMeshProUGUI _costText;
    private bool _tryToBuy;

    private void Update() {
        if(Input.GetKeyDown(KeyCode.E)) {
            _tryToBuy = true;
        } else if(Input.GetKeyUp(KeyCode.E)) {
            _tryToBuy = false;
        }
    }

    private void OnTriggerStay(Collider other) {
        if(other.CompareTag("Buyable")) {
            Buyable buyable = other.GetComponent<Buyable>();
            _costText.text = buyable.GetShown(_playerScripts);
            _costShower.SetActive(_costText.text!="");
            if(_tryToBuy) {
                buyable.Buy(_playerScripts);
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
