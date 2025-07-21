using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdMovement : MonoBehaviour{
    private Rigidbody2D rb;
    public float speed;

    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null){
            Debug.Log("The bird prefab doesn't have RigidBody");
        }
      
    }
    private void FixedUpdate() {
        rb.velocity = Vector2.left * speed;
    }
}
