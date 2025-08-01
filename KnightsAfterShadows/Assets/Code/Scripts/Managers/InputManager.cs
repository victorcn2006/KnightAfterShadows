using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour {
    [SerializeField] private InputActionAsset inputs;
    public InputAction leftClick { get; private set; }
    public InputAction navigate { get; private set; }
    public bool rightNavigation { get; private set; }

    public bool leftNavigation { get; private set; }
    private void Awake() {
        if (inputs == null)
        {
            Debug.LogError("InputManager: InputActionAsset not assigned");
            return;
        }
        //INPUT ACTIONS
        leftClick = inputs.FindActionMap("UI")?.FindAction("Click");
        navigate = inputs.FindActionMap("UI")?.FindAction("Navigate");

        if (leftClick == null) {
            Debug.LogError("InputManager:left Click was not found in the UI mapping");
            return;
        }
        if (navigate == null){
            Debug.Log("There's no right navigation action");
            return;
        }

        leftClick.Enable();
        navigate.Enable();
        navigate.performed += OnNavigate;
        navigate.canceled += ctx => {
            rightNavigation = false;
            leftNavigation = false;
        };
    }

    private void OnDisable() {
        if (leftClick != null)
            leftClick.Disable();

        if (navigate != null)
        {
            navigate.performed -= OnNavigate;
            navigate.Disable();
        }
    }
    private void OnNavigate(InputAction.CallbackContext ctx) {
        Vector2 direction = ctx.ReadValue<Vector2>();
        rightNavigation = direction.x > 0.5f;
        leftNavigation = direction.x < -0.5f;
    }
}
