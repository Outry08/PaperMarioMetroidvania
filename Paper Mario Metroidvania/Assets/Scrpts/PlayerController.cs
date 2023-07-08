using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerController : MonoBehaviour
{
    public Rigidbody2D rb;
    public InputAction playerMovement;
    public Animator animator;
    public CapsuleCollider2D bodyCollider;
    public BoxCollider2D footCollider;

    Vector2 moveDirection = Vector2.zero;
    Vector2 facingLeft;
    Vector2 facingRight;

    public float moveSpeed = 5f;
    public float jumpSpeed = 10f;

    //float xStandBox = 1.318955f;
    //float yStandBox = 2.162776f;
    //float xStandOff = -0.01298897f;
    //float yStandOff = 0.02544008f;
    float xStandBox;
    float yStandBox;
    float xStandOff;
    float yStandOff;
    float yCrouchBox = 1.783723f;
    float yCrouchOff = 0.0507103f;
    //float yFootStandOff = -1.268088f;
    float yFootStandOff;
    float yFootCrouchOff = -1.011846f;

    bool onGround = false;
    bool isFacingLeft = false;
    bool isCrouching = false;

    private void OnEnable()
    {
        playerMovement.Enable();
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

        xStandBox = bodyCollider.size.x;
        yStandBox = bodyCollider.size.y;
        xStandOff = bodyCollider.offset.x;
        yStandOff = bodyCollider.offset.y;
        yFootStandOff = footCollider.offset.y;
    }

    // Update is called once per frame
    void Update()
    {
        horizontalMovement();

        jump();

        crouch();

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //if(collision.collider.GetType() == typeof(BoxCollider2D) && collision.gameObject.tag == "Ground")
        //{
        //    onGround = true;
        //}
        Collider2D collided = collision.collider;
        
        if(footCollider.IsTouching(collided) && collided.gameObject.tag == "Ground") {
            onGround = true;
            Debug.Log("On the ground");
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        //if (collision.collider.GetType() == typeof(BoxCollider2D) && collision.gameObject.tag == "Ground")
        //{
        //    onGround = false;
        //}


        if (onGround && collision.gameObject.tag == "Ground")
        {
            onGround = false;
            Debug.Log("Off the ground");
        }
    }

    private void horizontalMovement()
    {
        //Moving left and right
        moveDirection = playerMovement.ReadValue<Vector2>();
        if (!isCrouching)
        {
            rb.velocity = new Vector2(moveDirection.x * moveSpeed, rb.velocity.y);
        }
        else if(isCrouching && rb.velocity.x > 0)
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
        }

        //Direction facing
        if (Input.GetKey(KeyCode.A) && moveDirection.x < 0)
        {
            isFacingLeft = true;
        }
        else if (Input.GetKey(KeyCode.D) && moveDirection.x > 0)
        {
            isFacingLeft = false;
        }

        if (isFacingLeft)
        {
            transform.localScale = facingLeft;
        }
        else if (!isFacingLeft)
        {
            transform.localScale = facingRight;
        }

        animator.SetFloat("xSpeed", Mathf.Abs(rb.velocity.x));

    }

    private void jump()
    {
        //Jumping
        if (Input.GetKeyDown(KeyCode.Space) && onGround)
        {
            rb.velocity = new Vector2(moveDirection.x * moveSpeed, jumpSpeed);
        }
        else if (Input.GetKeyUp(KeyCode.Space) && rb.velocity.y > 0)
        {
            rb.velocity = new Vector2(moveDirection.x * moveSpeed, 1);
        }

        animator.SetFloat("ySpeed", rb.velocity.y);

    }

    private void crouch()
    {
        //Crouching
        if (Input.GetKey(KeyCode.S) && onGround)
        {
            if (!isCrouching)
            {
                bodyCollider.size = new Vector2(xStandBox, yCrouchBox);
                bodyCollider.offset = new Vector2(xStandOff, yCrouchOff);
                footCollider.offset = new Vector2(xStandOff, yFootCrouchOff);
                rb.position = new Vector2(rb.position.x, rb.position.y + (yFootStandOff - yFootCrouchOff + footCollider.size.y/2));
            }
            isCrouching = true;
        }
        else
        {
            if (isCrouching)
            {
                bodyCollider.size = new Vector2(xStandBox, yStandBox);
                bodyCollider.offset = new Vector2(xStandOff, yStandOff);
                footCollider.offset = new Vector2(xStandOff, yFootStandOff);
                rb.position = new Vector2(rb.position.x, rb.position.y - (yFootStandOff - yFootCrouchOff + 0.12f));
                //0.12 because it's the height of the foot collider rounded up to keep Mario on the ground.
            }
            isCrouching = false;
        }

        animator.SetBool("isCrouching", isCrouching);

    }
}
