using System;
using System.Security.Cryptography;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AuthRouter : MonoBehaviour
{
    [Header("User Inputs")]
    [SerializeField] private TMP_InputField userInput;
    [SerializeField] private TMP_InputField passwordInput;

    [Header("Login/Register Button")]
    [SerializeField] private Button loginButton;
    [SerializeField] private Button registerButton;

    [Header("Toast input")]
    [SerializeField] private TMP_Text feedbackText;

    
    private void OnEnable() {
        loginButton.onClick.AddListener(OnLogin);
        registerButton.onClick.AddListener(OnRegister);
    }
    
    
    private void OnDisable() {
        loginButton.onClick.RemoveListener(OnLogin);
        registerButton.onClick.RemoveListener(OnRegister);
    }

    private void OnLogin() {
        if (!Validate(out string user, out string hash)) return;

        int userId = SQLiteReader.instance.ValidateUser(user, hash);
        if (userId == -1) {
            feedbackText.text = "Wrong username or password.";
            return;
        }

        UserSession.Login(userId, user);
        InventoryManager.instance.LoadInventoryForUser(userId);
        SceneManager.LoadScene("World");
        Debug.Log($"[Auth] User {user} logged in successfully.");
    }

    private void OnRegister() {
        if (!Validate(out string user, out string hash)) return;
        if (SQLiteReader.instance != null && SQLiteReader.instance.UsernameExists(user)) {
            feedbackText.text = "Username already taken.";
            return;
        }
        bool success = SQLiteReader.instance.RegisterUser(user, hash);
        feedbackText.text = success ? "Account created! You can now log in." : "Registration failed.";
    }
    
    private bool Validate(out string username, out string passwordHash) {
        username = userInput.text.Trim();
        passwordHash = "";
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(passwordInput.text)) {
            feedbackText.text = "Please fill in all fields.";
            return false;
        }
        if (passwordInput.text.Length < 6) {
            feedbackText.text = "Password must be at least 6 characters.";
            return false;
        }
        passwordHash = HashPassword(passwordInput.text);
        return true;
    }
    private string HashPassword(string raw) {
        using (var sha = SHA256.Create()) {
            byte[] bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(raw));
            var sb = new StringBuilder();
            foreach (byte b in bytes) sb.Append(b.ToString("x2"));
            return sb.ToString();
        }
    }
    
}
