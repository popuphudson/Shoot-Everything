using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public enum Stance {
    STANDING,
    CROUCHING,
    PRONE
}

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private PlayerInput _playerInput;
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private PlayerLook _playerLook;
    [SerializeField] private PlayerHealth _playerHealth;
    [SerializeField] private PlayerPerks _playerPerks;
    [SerializeField] private PauseMenu _pauser;
    [SerializeField] private GunHandler _gunHandler; 
    [HideInInspector] public AreaData CurrentArea;
    [SerializeField] private float _moveSmoothTime;
    [SerializeField] private float _gravityStrength;
    [SerializeField] private float _jumpStrength;
    [SerializeField] private float _proneSpeed;
    [SerializeField] private float _crouchSpeed;
    [SerializeField] private float _walkSpeed;
    [SerializeField] private float _runSpeed;
    [SerializeField] private float _defaultRunDecay;
    [SerializeField] private float _defaultRunRecharge;
    [SerializeField] private float _stanceChangeSmoothness; 
    [SerializeField] private float _slideTime;

    private CharacterController _controller;
    private Vector3 _currentMoveVelocity;
    private Vector3 _moveDampVelocity;
    private Vector3 _currentForceVelocity;
    private Vector3 _appliedForceVelocity;
    private Vector3 _storedForceVelocity;

    private float _jumpMul;
    private float _runTimer;
    private bool _running;
    private bool _holdingRunKey;
    private bool _runRecharge;
    private bool _frozen;

    private Stance _stance;
    private Vector3 _targetScale;
    private float _slideTimer;

    private InputAction _sprintInput;
    private InputAction _changeStanceInput;
    private InputAction _moveInput;
    private InputAction _jumpInput;

    private void Start() {
        _controller = GetComponent<CharacterController>();
        _jumpMul = 1;
        _stance = Stance.STANDING;
        _moveInput = _playerInput.actions["Move"];
        _sprintInput = _playerInput.actions["Sprint"];
        _changeStanceInput = _playerInput.actions["ChangeStance"];
        _jumpInput = _playerInput.actions["Jump"];
    }

    public Vector3 GetMoveVelocity() {
        return _currentMoveVelocity;
    }

    public void Freeze() {
        _frozen = true;
        _playerLook.Freeze();
    }

    public void UnFreeze() {
        _frozen = false;
        _playerLook.UnFreeze();
    }

    public void Teleport(Vector3 __location) {
        _controller.enabled = false;
        transform.localPosition = __location;
        _controller.enabled = true;
    }

    public bool IsRunning() {
        return _running;
    }

    private void Update() {
        if(_pauser.Paused) return;
        if(_frozen) return;
        Vector3 playerInput = new Vector3() {
            x = _moveInput.ReadValue<Vector2>().x,
            y = 0f,
            z = _moveInput.ReadValue<Vector2>().y
        };
        
        playerInput.Normalize();

        Vector3 moveVector = transform.TransformDirection(playerInput);

        if(!_holdingRunKey && _runTimer > 0 && !_runRecharge) {
            _holdingRunKey = _sprintInput.WasPressedThisFrame();
        } else if(_runTimer <= 0 || _sprintInput.WasReleasedThisFrame()) {
            _holdingRunKey = false;
            _runRecharge = true;
        }
        if(_runRecharge && _runTimer == 1) _runRecharge = false;
        _running = _holdingRunKey && _slideTimer <= 0 && (playerInput.z > 0 || _playerPerks.HasPerks(Perks.BETTER_RUN) || _playerPerks.HasSideMixPerk(Perks.BETTER_RUN) || _playerPerks.HasMainMixPerk(Perks.BETTER_RUN));
        if(_running) {
            if(_slideTimer <= 0) {
                _stance = Stance.STANDING;
            }
            _runTimer -= Time.deltaTime*_defaultRunDecay/((_playerPerks.HasPerks(Perks.BETTER_RUN)?4:1)*(_playerPerks.HasMainMixPerk(Perks.BETTER_RUN)?3:1)*(_playerPerks.HasSideMixPerk(Perks.BETTER_RUN)?1.75f:1));
            _runTimer = Mathf.Max(0, _runTimer);
        } else {
            _runTimer += Time.deltaTime*_defaultRunRecharge*((_playerPerks.HasPerks(Perks.BETTER_RUN)?2:1)*(_playerPerks.HasMainMixPerk(Perks.BETTER_RUN)?1.5f:1));
            _runTimer = Mathf.Min(_runTimer, 1);
        }

        if(_changeStanceInput.WasPressedThisFrame()) {
            if(_stance == Stance.STANDING) {
                _stance = Stance.CROUCHING;
            } else if(_stance == Stance.CROUCHING){
                _stance = Stance.PRONE;
            } else {
                _stance = Stance.STANDING;
            }
        }

        if(_slideTimer > 0 && _slideTimer-Time.deltaTime < 0) {
            _stance = Stance.STANDING;
        }
        _slideTimer = Mathf.Max(_slideTimer-Time.deltaTime, -0.5f);
        if(_running && _stance == Stance.CROUCHING && _slideTimer <= 0) {
            _slideTimer = _slideTime;
        }

        float currentSpeed = _running? _runSpeed : (_stance==Stance.STANDING? _walkSpeed : (_stance==Stance.CROUCHING? _crouchSpeed : _proneSpeed));
        currentSpeed *= _playerPerks.HasMix(Perks.BETTER_RUN, Perks.EXTRA_HEALTH)?Mathf.Max(_playerHealth.GetHealth()/125f, 1):1;
        currentSpeed *= _gunHandler.GetAiming()?0.5f:1;
        _currentMoveVelocity = Vector3.SmoothDamp(
            _currentMoveVelocity,
            moveVector * currentSpeed * (_playerPerks.HasPerks(Perks.BETTER_RUN)?1.125f:1),
            ref _moveDampVelocity,
            _moveSmoothTime
        );

        Ray groundCheckRay = new Ray(transform.position, Vector3.down);
        if(Physics.Raycast(groundCheckRay, 1.1f, _groundLayer)) {
            
            if(_running && _stance == Stance.CROUCHING) {
                _running = false;
                _stance = Stance.PRONE;
                _appliedForceVelocity = _currentMoveVelocity*1.25f;
                _storedForceVelocity = _currentMoveVelocity*1.25f;
            } else if(_slideTimer <= 0) {
                _appliedForceVelocity = _currentMoveVelocity;
                _storedForceVelocity = _currentMoveVelocity;
            }
            

            if(_currentForceVelocity.y <= 0 && _slideTimer < 0) {
                _currentForceVelocity.y = -2f;
            } else if(_currentForceVelocity.y <= 0) {
                _currentForceVelocity.y = -10f;
            }

            if(_jumpInput.WasPressedThisFrame() && (_stance == Stance.STANDING || _slideTimer > 0) && _jumpMul > 0.5f) {
                _currentForceVelocity.y = _jumpStrength*_jumpMul;
                _jumpMul /= 1.1f;
                if(_jumpMul < 0.5f) {
                    _jumpMul = 0f;
                }
                _appliedForceVelocity = _appliedForceVelocity*0.75f;
                _storedForceVelocity = _storedForceVelocity*0.75f;
                if(_slideTimer > 0) {
                    _slideTimer = 0.1f;
                }
                _stance = Stance.STANDING;
            } else {
                _jumpMul = Mathf.Clamp(_jumpMul+(Time.deltaTime/2f), 0, 1);
            }
        } else {
            _currentForceVelocity.y -= _gravityStrength * _gravityStrength * Time.deltaTime;
            _appliedForceVelocity = _storedForceVelocity+(_currentMoveVelocity/5f);
        }

        _controller.Move(_appliedForceVelocity * Time.deltaTime);

        _controller.Move(_currentForceVelocity * Time.deltaTime);

        if(_stance == Stance.STANDING) {
            _targetScale = Vector3.one;
        } else if(_stance == Stance.CROUCHING) {
            _targetScale = new Vector3(1, 0.5f, 1);
        } else {
            _targetScale = new Vector3(0.5f, 0.2f, 0.5f);
        }
        transform.localScale = Vector3.Lerp(transform.localScale, _targetScale, (_slideTimer>0?10:1)*Time.deltaTime*_stanceChangeSmoothness);

    }

    private void OnTriggerEnter(Collider __other) {
        if(__other.CompareTag("Area")) {
            CurrentArea = __other.GetComponent<AreaData>();
        }
    }
}
