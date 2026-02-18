using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AuthRouter : MonoBehaviour
{
    [SerializeField] private TMP_InputField userInput;
    [SerializeField] private TMP_InputField passwordInput;

    [SerializeField] private Button loginButton;
    [SerializeField] private Button registerButton;

    /*
    private void OnEnable() {
        userInput.onValueChanged.AddListener(CheckUser);
        passwordInput.onValueChanged.AddListener(CheckPassword);
    }
    */
    /*
    private void OnDisable() {
        userInput.onValueChanged.RemoveListener(CheckUser);
        passwordInput.onValueChanged.RemoveListener(CheckPassword);
    }
    */
}
