using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerController : MonoBehaviour
{
    public Rigidbody2D rb;
    public InputAction playerMovement;
    //public InputAction playerJump;
    Vector2 moveDirection = Vector2.zero;
    Vector2 facingLeft;
    Vector2 facingRight;
    public float moveSpeed = 5f;
    public float jumpSpeed = 10f;

    bool onGround = false;
    bool isFacingLeft = false;

    private void OnEnable()
    {
        playerMovement.Enable();
        //playerJump.Enable();
    }

    private void OnDisable()
    {
        playerMovement.Disable();
        //playerJump.Disable();
    }

    // Start is called before the first frame update
    void Start()
    {
        facingLeft = new Vector2(-transform.localScale.x, transform.localScale.y);
        facingRight = new Vector2(transform.localScale.x, transform.localScale.y);
    }

    // Update is called once per frame
    void Update()
    {
        //Moving left and right
        moveDirection = playerMovement.ReadValue<Vector2>();
        rb.velocity = new Vector2(moveDirection.x * moveSpeed, rb.velocity.y);
        if (Input.GetKeyDown(KeyCode.A))
        {
            isFacingLeft = true;
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            isFacingLeft = false;
        }

        if (isFacingLeft)
        {
            transform.localScale = facingLeft;
        }
        if (!isFacingLeft)
        {
            transform.localScale = facingRight;
        }

        //Jumping
        if (Input.GetKeyDown(KeyCode.Space) && onGround)
        {
            rb.velocity = new Vector2(moveDirection.x * moveSpeed, jumpSpeed);
        }
        else if (Input.GetKeyUp(KeyCode.Space) && rb.velocity.y > 0)
        {
            rb.velocity = new Vector2(moveDirection.x * moveSpeed, 1);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.name == "Ground")
        {
            onGround = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.name == "Ground")
        {
            onGround = false;
        }
    }
}
