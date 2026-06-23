using UnityEngine;

public class PlayerStats: MonoBehaviour
{
    [Header("Perks")]
    public int health;
    [Header("Movement")]
    public float speed;
    public float dashSpeed;
    [Header("Dash Movement")]
    public float dashDuration;
    public float dashCooldown;
}
