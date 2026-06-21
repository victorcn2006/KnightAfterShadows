using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private InputActionAsset _controls;
    private Rigidbody2D _rb;
    private Vector2 _moveInput;
    private PlayerStats _playerStats;
    private InputAction _moveAction;

    private void Awake() {
        _rb = GetComponent<Rigidbody2D>();
        _playerStats = GetComponent<PlayerStats>();
        if (!_controls) {
            Debug.LogWarning("Controls input action asset is null");
            return;
        }
        _moveAction = _controls.FindAction("Move");
    }

    private void OnEnable() {
        _moveAction.Enable();
    }

    private void OnDisable() {
        _moveAction.Disable();
    }

    private void FixedUpdate() {
        Vector2 moveInput = _moveAction.ReadValue<Vector2>();
        _rb.linearVelocity = moveInput * _playerStats.speed;
    }
}
