using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class DashController : MonoBehaviour
{
    private Player _player;
    private PlayerStats _playerStats;
    private PlayerMovement _playerMovement;
    private Rigidbody2D _rb;

    private InputAction _dashAction;

    private bool _canDash = true;
    public bool IsDashing { get; private set; }



    private void Awake() {
        _player = GetComponent<Player>();
        _playerStats = GetComponent<PlayerStats>();
        _playerMovement = GetComponent<PlayerMovement>();
        _rb = GetComponent<Rigidbody2D>();

        _dashAction = _player._controls.FindAction("Dash");
    }

    private void OnEnable() {
        _dashAction.Enable();
        _dashAction.performed += OnDash;
    }

    private void OnDisable() {     
        _dashAction.performed -= OnDash;
        _dashAction.Disable();
    }

    private void OnDash(InputAction.CallbackContext ctx) {
        if (!_canDash || IsDashing)
            return;

        StartCoroutine(DashCoroutine());
    }

    private IEnumerator DashCoroutine() {
        _canDash = false;
        IsDashing = true;

        Vector2 dashDir = _playerMovement.GetLastDirection();


        float elapsed = 0f;

        while (elapsed < _playerStats.dashDuration) {
            _rb.linearVelocity = dashDir * _playerStats.dashSpeed;

            elapsed += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        IsDashing = false;

        yield return new WaitForSeconds(_playerStats.dashCooldown);

        _canDash = true;
    }
}
