using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    Vector2 moveInput;
    Vector2 jumpInput;
    Rigidbody2D rb;
    [SerializeField] float reloadSceneTimer;
    [SerializeField] float forceAddedAfterEnemyKill;
    [SerializeField] float runSpeed = 5000f;
    [SerializeField] float climbSpeed = 5f;
    [SerializeField] float jumpPower = 5f;
    float playerGravictyScale;
    BoxCollider2D boxCollider2D;
    CapsuleCollider2D capsuleCollider2D;
    Animator animator;
    bool isPlayerRunning;
    bool isPlayerClimbing;

    private void Start() {
        boxCollider2D = GetComponent<BoxCollider2D>();
        capsuleCollider2D = GetComponent<CapsuleCollider2D>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        playerGravictyScale = rb.gravityScale;
    }

    private void Update() {
        Run();
        FlipSprite();
        ClimbLadder();
    }

    void OnMove(InputValue value) {
        moveInput = value.Get<Vector2>();
        Debug.Log(moveInput);
    }

    void Run() {
        Vector2 playerVelocity = new Vector2(moveInput.x * runSpeed, rb.velocity.y);
        rb.velocity = playerVelocity;
        isPlayerRunning = Mathf.Abs(rb.velocity.x) > Mathf.Epsilon;
        animator.SetBool("isRunning", isPlayerRunning);
    }

    void FlipSprite() {
        if (isPlayerRunning) {
            transform.localScale = new Vector2(Mathf.Sign(rb.velocity.x), 1f);
        }
    }

    void OnJump(InputValue value) {
        if(!boxCollider2D.IsTouchingLayers(LayerMask.GetMask("Ground"))) { return; }
        if(value.isPressed) {
            rb.AddForce(new Vector2(0f, jumpPower));
            Debug.Log("Jumping!");
        }
    }

    void ClimbLadder() {
        isPlayerClimbing = Mathf.Abs(rb.velocity.y) > Mathf.Epsilon;
        if(!capsuleCollider2D.IsTouchingLayers(LayerMask.GetMask("Climbing"))) { 
            animator.SetBool("isClimbing", false);
            rb.gravityScale = playerGravictyScale;
            return; 
        }
        rb.gravityScale = 0;
        Vector2 climbVelocity = new Vector2(rb.velocity.x, moveInput.y * climbSpeed);
        rb.velocity = climbVelocity;
        animator.SetBool("isClimbing", isPlayerClimbing);
    }

    private void OnCollisionEnter2D(Collision2D other) {
        if (boxCollider2D.IsTouchingLayers(LayerMask.GetMask("Enemy"))) {
            Destroy(other.gameObject);
            rb.AddForce(Vector2.up * forceAddedAfterEnemyKill);
        } else if(capsuleCollider2D.IsTouchingLayers(LayerMask.GetMask("Enemy"))) {
            StartCoroutine(DeathSequence(reloadSceneTimer));
        }
    }

    IEnumerator DeathSequence(float reloadSceneTimer) {
        GetComponent<SpriteRenderer>().enabled = false;
        boxCollider2D.enabled = false;
        capsuleCollider2D.enabled = false;
        runSpeed = 0;
        jumpPower = 0;
        climbSpeed = 0;
        yield return new WaitForSeconds(reloadSceneTimer);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
