using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private PauseMenu _pauser;
    [HideInInspector] public AreaData CurrentArea;
    [SerializeField] private float _moveSmoothTime;
    [SerializeField] private float _gravityStrength;
    [SerializeField] private float _jumpStrength;
    [SerializeField] private float _walkSpeed;
    [SerializeField] private float _runSpeed;
    [SerializeField] private float _defaultRunDecay;
    [SerializeField] private float _defaultRunRecharge;
    [SerializeField] private PlayerPerks _playerPerks;

    private CharacterController _controller;
    private Vector3 _currentMoveVelocity;
    private Vector3 _moveDampVelocity;
    private Vector3 _currentForceVelocity;

    private float _runTimer;
    private bool _running;
    private bool _holdingRunKey;
    private bool _runRecharge;

    private void Start() {
        _controller = GetComponent<CharacterController>();
    }

    public bool IsRunning() {
        return _running;
    }

    private void Update() {
        if(_pauser.Paused) return;
        Vector3 playerInput = new Vector3() {
            x = Input.GetAxisRaw("Horizontal"),
            y = 0f,
            z = Input.GetAxisRaw("Vertical")
        };
        
        playerInput.Normalize();

        Vector3 moveVector = transform.TransformDirection(playerInput);
        if(!_holdingRunKey && _runTimer > 0 && !_runRecharge) {
            _holdingRunKey = Input.GetKeyDown(KeyCode.LeftShift);
        } else if(_runTimer <= 0 || Input.GetKeyUp(KeyCode.LeftShift)) {
            _holdingRunKey = false;
            _runRecharge = true;
        }
        if(_runRecharge && _runTimer == 1) _runRecharge = false;
        _running = _holdingRunKey && playerInput.z > 0;
        if(_running) {
            _runTimer -= (Time.deltaTime*_defaultRunDecay)/(_playerPerks.HasPerks(Perks.BETTER_RUN)?4:1);
            _runTimer = Mathf.Max(0, _runTimer);
        } else {
            _runTimer += Time.deltaTime*_defaultRunRecharge*(_playerPerks.HasPerks(Perks.BETTER_RUN)?2:1);
            _runTimer = Mathf.Min(_runTimer, 1);
        }
        float currentSpeed = _running? _runSpeed : _walkSpeed;
        _currentMoveVelocity = Vector3.SmoothDamp(
            _currentMoveVelocity,
            moveVector * currentSpeed * (_playerPerks.HasPerks(Perks.BETTER_RUN)?1.125f:1),
            ref _moveDampVelocity,
            _moveSmoothTime
        );

        _controller.Move(_currentMoveVelocity * Time.deltaTime);

        Ray groundCheckRay = new Ray(transform.position, Vector3.down);
        if(Physics.Raycast(groundCheckRay, 1.1f)) {
            
            if(_currentForceVelocity.y <= 0) {
                _currentForceVelocity.y = -2f;
            } 

            if(Input.GetKeyDown(KeyCode.Space)) {
                _currentForceVelocity.y = _jumpStrength;
            }
        } else {
            _currentForceVelocity.y -= _gravityStrength * _gravityStrength * Time.deltaTime;
        }

        _controller.Move(_currentForceVelocity * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other) {
        if(other.CompareTag("Area")) {
            CurrentArea = other.GetComponent<AreaData>();
        }
    }
}
