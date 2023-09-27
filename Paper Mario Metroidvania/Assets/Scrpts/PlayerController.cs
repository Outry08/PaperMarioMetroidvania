using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;


public class PlayerController : MonoBehaviour
{
    public Rigidbody2D rb;
    public InputAction playerMovement;
    public Animator animator;
    public BoxCollider2D bodyCollider;
    public BoxCollider2D footCollider;
    public BoxCollider2D headCollider;

    public TextTracker damageText;

    Vector2 moveDirection = Vector2.zero;
    Vector2 facingLeft;
    Vector2 facingRight;

    public float moveSpeed = 5f;
    public float jumpSpeed = 10f;

    public int health = 10;
    public int atk = 1;

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
    float yHeadStandOff;
    float yHeadCrouchOff = 1f;

    int stunTimer;
    int iFrames;

    bool onGround = false;
    bool isFacingLeft = false;
    bool isCrouching = false;
    bool touchingWall = false, wallIsLeft;

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
        yHeadStandOff = headCollider.offset.y;
    }

    // Update is called once per frame
    void Update()
    {

        if(stunTimer > 0)
        {
            stunTimer--;
            animator.SetInteger("stunTime", stunTimer);
        }
        if(iFrames > 0)
        {
            iFrames--;
        }


        if(stunTimer == 0)
        {
            horizontalMovement();

            jump();

            crouch();
        }

        if (health <= 0)
        {
            this.gameObject.SetActive(false);
        }

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {

        Collider2D collided = collision.collider;
        int knockX = 7;
        int knockY = 8;
        int highBounce = 15;
        int lowBounce = 8;

        //Ground + Foot collision
        if(footCollider.IsTouching(collided) && collided.gameObject.tag == "Ground") {
            onGround = true;
            animator.SetBool("onGround", onGround);
            Debug.Log("On the ground");
        }

        //Jumping off of an enemy
        if (footCollider.IsTouching(collided) && collided.gameObject.tag == "Enemy" && collided.GetType() == typeof(BoxCollider2D))
        {
            Enemy enemy = collision.gameObject.GetComponent<Enemy>();

            //If holding space, bounce higher
            if (Input.GetKey(KeyCode.Space))
                rb.velocity = new Vector2(rb.velocity.x, highBounce);
            //Else, don't bounce as high
            else
                rb.velocity = new Vector2(rb.velocity.x, lowBounce);

            Debug.Log("Bounce!");

            enemy.takeDamage(atk);
            damageText.showDamage(footCollider.transform.position, atk, 'y');
        }

        //Taking damage from an enemy
        if ((bodyCollider.IsTouching(collided) || headCollider.IsTouching(collided)) && collided.gameObject.tag == "Enemy")
        {
            Enemy enemy = collision.gameObject.GetComponent<Enemy>();

            if (collided.transform.position.x < this.transform.position.x)
            {
                rb.velocity = new Vector2(knockX, knockY);
            }
            else
            {
                rb.velocity = new Vector2(-knockX, knockY);
            }

            stunTimer = 150;
            iFrames = stunTimer * 2;

            health -= enemy.getAtk();
            damageText.showDamage(bodyCollider.transform.position, atk, 'r');

            Debug.Log("OUCH!\nHEATLH: " + health);
        }

        //Wall collision
        if(collided.gameObject.tag.Equals("Wall"))
        {
            touchingWall = true;
            if (collided.transform.position.x < this.transform.position.x)
                wallIsLeft = true;
            else
                wallIsLeft = false;
            Debug.Log("Is wall left?   " + wallIsLeft);
        }


    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        //Ground + Foot collision
        if (onGround && collision.gameObject.tag == "Ground")
        {
            onGround = false;
            animator.SetBool("onGround", onGround);
            Debug.Log("Off the ground");
        }

        if (collision.gameObject.tag.Equals("Wall"))
            touchingWall = false;
    }

    private void horizontalMovement()
    {
        //Moving left and right
        moveDirection = playerMovement.ReadValue<Vector2>();

        if(touchingWall && ((moveDirection.x < 0 && wallIsLeft) || (moveDirection.x > 0 && !wallIsLeft)))
            moveDirection.x = 0;

        if (!isCrouching)
            rb.velocity = new Vector2(moveDirection.x * moveSpeed, rb.velocity.y);
        else if(isCrouching && rb.velocity.x > 0)
            rb.velocity = new Vector2(0, rb.velocity.y);

        //Direction facing
        if (Input.GetKey(KeyCode.A) && moveDirection.x < 0)
            isFacingLeft = true;
        else if (Input.GetKey(KeyCode.D) && moveDirection.x > 0)
            isFacingLeft = false;

        if (isFacingLeft)
            transform.localScale = facingLeft;
        else if (!isFacingLeft)
            transform.localScale = facingRight;

        animator.SetFloat("xSpeed", Mathf.Abs(rb.velocity.x));

    }

    private void jump()
    {
        //Jumping
        if (Input.GetKeyDown(KeyCode.Space) && onGround)
            rb.velocity = new Vector2(moveDirection.x * moveSpeed, jumpSpeed);
        else if (Input.GetKeyUp(KeyCode.Space) && rb.velocity.y > 0)
            rb.velocity = new Vector2(moveDirection.x * moveSpeed, 1);

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

                headCollider.offset = new Vector2(xStandOff, yHeadCrouchOff);

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

                headCollider.offset = new Vector2(xStandOff, yHeadStandOff);

                rb.position = new Vector2(rb.position.x, rb.position.y - (yFootStandOff - yFootCrouchOff + 0.12f));
                //0.12 because it's the height of the foot collider rounded up to keep Mario on the ground.
            }
            isCrouching = false;
        }

        animator.SetBool("isCrouching", isCrouching);

    }

    public int getHealth()
    {
        return health;
    }
    public void setHealth(int num)
    {
        health = num;
    }
}
