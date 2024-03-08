using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] float movementSpeed = 1f;
    Rigidbody2D rb;

    private void Start() {
        rb = GetComponent<Rigidbody2D>();
    }
    private void Update() {
        rb.velocity = new Vector2(movementSpeed, 0f);
    }

    private void OnTriggerExit2D(Collider2D other) {
        Debug.Log("I exited collision");
        movementSpeed = -movementSpeed;
        transform.localScale = new Vector2(-(Mathf.Sign(rb.velocity.x)), 1f);
    }
}
