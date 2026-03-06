using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class AuthRouter : MonoBehaviour
{
    [SerializeField] private TMP_InputField userInput;
    [SerializeField] private TMP_InputField passwordInput;

    [SerializeField] private Button loginButton;
    [SerializeField] private Button registerButton;

    [SerializeField] private string successScene = "Menu";

    private void OnEnable() {
        if (loginButton != null) loginButton.onClick.AddListener(OnLoginClicked);
        if (registerButton != null) registerButton.onClick.AddListener(OnRegisterClicked);
    }
    
    private void OnDisable() {
        if (loginButton != null) loginButton.onClick.RemoveListener(OnLoginClicked);
        if (registerButton != null) registerButton.onClick.RemoveListener(OnRegisterClicked);
    }

    private void OnLoginClicked() {
        string user = userInput.text;
        string pass = passwordInput.text;

        if (string.IsNullOrEmpty(user) || string.IsNullOrEmpty(pass)) {
            Debug.LogWarning("Username and Password cannot be empty.");
            return;
        }

        if (SQLiteReader.instance.ValidateUser(user, pass)) {
            Debug.Log("Login Successful!");
            if (Systems.instance != null) Systems.instance.CurrentUsername = user;
            SceneManager.LoadScene(successScene);
        } else {
            Debug.LogError("Login Failed: Invalid username or password.");
        }
    }

    private void OnRegisterClicked() {
        string user = userInput.text;
        string pass = passwordInput.text;

        if (string.IsNullOrEmpty(user) || string.IsNullOrEmpty(pass)) {
            Debug.LogWarning("Username and Password cannot be empty.");
            return;
        }

        if (SQLiteReader.instance.RegisterUser(user, pass)) {
            Debug.Log("Registration Successful!");
        } else {
            Debug.LogError("Registration Failed: Username might already exist.");
        }
    }

    // Placeholders for the listeners mentioned in the previous version
    private void CheckUser(string value) { }
    private void CheckPassword(string value) { }
}
