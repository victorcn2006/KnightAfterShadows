using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserSession : MonoBehaviour
{
    public static int UserId { get; private set; } = -1;
    public static string Username { get; private set; } = "";
    public static bool IsLoggedIn => UserId != -1;
    public static void Login(int userId, string username) {
        UserId = userId;
        Username = username;
    }
    public static void Logout() {
        UserId = -1;
        Username = "";
    }
}
