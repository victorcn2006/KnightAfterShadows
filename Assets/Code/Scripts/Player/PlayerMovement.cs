using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour {
    [SerializeField] private InputActionAsset _controls;

    private Rigidbody2D _rb;
    private PlayerStats _playerStats;

    private InputAction _moveAction;
    private InputAction _dashAction;

    private Vector2 _moveInput;
    private Vector2 _lastDirection = Vector2.right;

    private bool _isDashing;
    private bool _canDash = true;

    private void Awake() {
        _rb = GetComponent<Rigidbody2D>();
        _playerStats = GetComponent<PlayerStats>();
        if (!_controls) {
            Debug.LogWarning("Controls input action asset is null");
            return;
        }
        _moveAction = _controls.FindAction("Move");
        _dashAction = _controls.FindAction("Dash");
    }

    private void OnEnable() {
        _moveAction.Enable();
        _dashAction.Enable();

        _dashAction.performed += OnDash;
    }

    private void OnDisable() {
        _dashAction.performed -= OnDash;

        _moveAction.Disable();
        _dashAction.Disable();
    }

    private void OnDash(InputAction.CallbackContext ctx) {
        if (!_canDash || _isDashing)
            return;

        StartCoroutine(DashCoroutine());
    }

    private IEnumerator DashCoroutine() {
        _canDash = false;
        _isDashing = true;

        Vector2 dashDir = _lastDirection;

        float elapsed = 0f;

        while (elapsed < _playerStats.dashDuration) {
            _rb.linearVelocity = dashDir * _playerStats.dashSpeed;

            elapsed += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        _isDashing = false;

        yield return new WaitForSeconds(_playerStats.dashCooldown);

        _canDash = true;
    }

    private void Update() {
        _moveInput = _moveAction.ReadValue<Vector2>();

        if (_moveInput != Vector2.zero)
            _lastDirection = _moveInput.normalized;
    }

    private void FixedUpdate() {
        if (_isDashing)
            return;

        _rb.linearVelocity = _moveInput * _playerStats.speed;
    }
}
