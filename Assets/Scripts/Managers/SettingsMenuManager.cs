using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsMenuManager : MonoBehaviour
{
    [SerializeField] private TMP_InputField _mouseXInput;
    [SerializeField] private Slider _mouseXSlider;
    [SerializeField] private TMP_InputField _mouseYInput;
    [SerializeField] private Slider _mouseYSlider;
    [SerializeField] private TMP_InputField _fovInput;
    [SerializeField] private Slider _fovSlider;
    [SerializeField] private Toggle _toggle;

    public void UpdateFOVInput() {
        float change = 0;
        if(float.TryParse(_fovInput.text, out change)) {
            _fovSlider.value = change;
        }
    }

    public void UpdateFOVSlider() {
        _fovInput.text = (Mathf.Round(_fovSlider.value*100)/100).ToString();
    }

    public void UpdateMouseXSensInput() {
        float change = 0;
        if(float.TryParse(_mouseXInput.text, out change)) {
            _mouseXSlider.value = change;
        }
    }

    public void UpdateMouseXSensSlider() {
        _mouseXInput.text = (Mathf.Round(_mouseXSlider.value*100)/100).ToString();
    }

    public void UpdateMouseYSensInput() {
        float change = 0;
        if(float.TryParse(_mouseYInput.text, out change)) {
            _mouseYSlider.value = change;
        }
    }

    public void UpdateMouseYSensSlider() {
        _mouseYInput.text = (Mathf.Round(_mouseYSlider.value*100)/100).ToString();
    }

    private void Start() {
        _mouseXInput.text = GlobalSettingsManager.Instance.MouseSensitivity.x.ToString();
        _mouseXSlider.value = GlobalSettingsManager.Instance.MouseSensitivity.x;
        _mouseYInput.text = GlobalSettingsManager.Instance.MouseSensitivity.y.ToString();
        _mouseYSlider.value = GlobalSettingsManager.Instance.MouseSensitivity.y;
        _fovInput.text = GlobalSettingsManager.Instance.FOV.ToString();
        _fovSlider.value = GlobalSettingsManager.Instance.FOV;
        _toggle.isOn = GlobalSettingsManager.Instance.ShowHealthBars;
    }

    private void Update() {
        GlobalSettingsManager.Instance.MouseSensitivity = new Vector2(_mouseXSlider.value, _mouseYSlider.value);
        GlobalSettingsManager.Instance.ShowHealthBars = _toggle.isOn;
        GlobalSettingsManager.Instance.FOV = _fovSlider.value;
    }
}
