using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour {

    private Player _player;
    private Rigidbody2D _rb;
    private PlayerStats _playerStats;
    private DashController _dashController;

    private InputAction _moveAction;
    private Vector2 _moveInput;
    private Vector2 _lastDirection = Vector2.right;

    private Animator _anim;



    private void Awake() {
        _rb = GetComponent<Rigidbody2D>();
        _playerStats = GetComponent<PlayerStats>();
        _player = GetComponent<Player>();
        _dashController = GetComponent<DashController>();
        _anim = GetComponentInChildren<Animator>();

        _moveAction = _player._controls.FindAction("Move");

    }

    private void OnEnable() {
        _moveAction.Enable();
    }

    private void OnDisable() {
        _moveAction.Disable();
    }

    private void Update() {
        _moveInput = _moveAction.ReadValue<Vector2>();

        if (_moveInput != Vector2.zero)
            _lastDirection = _moveInput.normalized;

        _anim.SetFloat("MoveX", _lastDirection.x);
        _anim.SetFloat("MoveY", _lastDirection.y);

        _anim.SetFloat("Speed", _moveInput.sqrMagnitude);
    }

    private void FixedUpdate() {
        if (_dashController.IsDashing)
            return;

        _rb.linearVelocity = _moveInput * _playerStats.speed;
    }

    public Vector2 GetLastDirection() {
        return _lastDirection;
    }
}