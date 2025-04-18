using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerLook : MonoBehaviour
{
    [SerializeField] private PlayerInput _playerInput;
    [SerializeField] private PauseMenu _pauser;
    [SerializeField] private Transform _playerCamera;
    [SerializeField] private Vector2 _sensitivity;
    [SerializeField] private Vector3 _cameraOffset;
    [SerializeField] private float _recoilMoveSpeed = 20;
    [SerializeField] private float _returnRecoilMoveSpeed = 20;
    private Vector2 _xyRotation;
    private Vector2 _lateRecoilRotation;
    private Vector2 _recoilRotation;
    private Vector2 _lateReversibleRecoilRotation;
    private Vector2 _reversibleRecoilRotation;
    private bool _allowPlayerInput;
    private InputAction _lookInput;

    public void Freeze() {
        _allowPlayerInput = false;
    }

    public void UnFreeze() {
        _allowPlayerInput = true;
    }

    public void SetRotation(Vector3 __eulerAngles) {
        _xyRotation = __eulerAngles;
    }

    public void AddRecoil(Vector2 __recoil) {
        _recoilRotation += __recoil;
    }

    public void AddReversibleRecoil(Vector2 __recoil) {
        _reversibleRecoilRotation += __recoil;
    }

    private void Start() {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        _sensitivity = GlobalSettingsManager.Instance.MouseSensitivity;
        _playerCamera.GetComponent<Camera>().fieldOfView = GlobalSettingsManager.Instance.FOV;
        _allowPlayerInput = true;
        _xyRotation = new Vector2(transform.eulerAngles.x, transform.eulerAngles.y);
        _lookInput = _playerInput.actions["Look"];
    }

    public void StopPlayerInput() {
        _allowPlayerInput = false;
    }

    public void UpdateSense() {
        _sensitivity = GlobalSettingsManager.Instance.MouseSensitivity;
    }

    public void UpdateFOV() {
        _playerCamera.GetComponent<Camera>().fieldOfView = GlobalSettingsManager.Instance.FOV;
    }

    private void Update() {
        if(_pauser.Paused) return;
        if(_allowPlayerInput) {
            Vector2 mouseInput = _lookInput.ReadValue<Vector2>()/15;
            _xyRotation.x -= mouseInput.y * _sensitivity.y;
            _xyRotation.y += mouseInput.x * _sensitivity.x;
        }

        Vector2 beforeLerpRecoil = _lateRecoilRotation;
        _lateRecoilRotation = Vector2.Lerp(_lateRecoilRotation, _recoilRotation, Time.deltaTime*_recoilMoveSpeed);
        Vector2 rotationDifference = _lateRecoilRotation-beforeLerpRecoil;

        Vector2 beforeLerpReversibleRecoil = _lateReversibleRecoilRotation;
        _lateReversibleRecoilRotation = Vector2.Lerp(_lateReversibleRecoilRotation, _reversibleRecoilRotation, Time.deltaTime*_recoilMoveSpeed);
        _reversibleRecoilRotation = Vector2.Lerp(_reversibleRecoilRotation, Vector2.zero, Time.deltaTime*_returnRecoilMoveSpeed);
        rotationDifference += _lateReversibleRecoilRotation-beforeLerpReversibleRecoil;

        _xyRotation += rotationDifference;

        _xyRotation.x = Mathf.Clamp(_xyRotation.x, -85f, 85f);

        transform.eulerAngles = new Vector3(0f, _xyRotation.y, 0f);
        _playerCamera.eulerAngles = new Vector3(_xyRotation.x, _xyRotation.y, 0f);
    }

    private void LateUpdate() {
        _playerCamera.position = transform.position + _cameraOffset;
    }
}
