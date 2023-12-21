using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;


public class PlayerController : MonoBehaviour
{
    public Rigidbody2D rb;
    public Transform transfrm;
    public InputAction playerMovement;
    public Animator animator;
    public BoxCollider2D bodyCollider;
    public BoxCollider2D footCollider;
    public BoxCollider2D headCollider;

    public TextTracker damageText;

    Vector2 moveDirection = Vector2.zero;
    Vector2 facingLeft;
    Vector2 facingRight;

    const float moveSpeed = 5f;
    const float jumpSpeed = 15f;

    int maxHealth = 10;
    int health = 10;
    int atk = 1;
    int numCoins = 0;

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

    int stunTimer = 0;
    int iFrames = 0;
    int deadTimer = 0;

    public bool onGround = false;
    //bool onPlat = false;
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
        if (isDead() && deadTimer == -1)
        {
            Debug.Log("DED");
            this.gameObject.SetActive(false);
        }

        if (!isDead())
        {
            if (stunTimer > 0)
            {
                stunTimer--;
                animator.SetInteger("stunTime", stunTimer);
            }
            if (iFrames > 0)
            {
                iFrames--;
                if (iFrames % 60 > 30)
                    animator.SetBool("isInvisible", true);
                else
                    animator.SetBool("isInvisible", false);
            }


            if (stunTimer == 0)
            {
                horizontalMovement();

                jump();

                crouch();
            }
        }

        if (isDead() && deadTimer >= 0)
        {
            Debug.Log(deadTimer);
            deadTimer--;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {

        Collider2D collided = collision.collider;
        int highBounce = 15;
        int lowBounce = 8;

        //Ground + Foot collision
        if (footCollider.IsTouching(collided) && collided.gameObject.tag == "Platform")
        {
            onGround = true;
            animator.SetBool("onGround", onGround);
            Debug.Log("On the ground");
        }
        //else if (footCollider.IsTouching(collided) && collided.gameObject.tag == "Platform")
        //{
        //    onPlat = true;
        //    animator.SetBool("onGround", onPlat);
        //    Debug.Log("On a platform");
        //}

        //Jumping off of an enemy
        else if (footCollider.IsTouching(collided) && collided.gameObject.tag == "Enemy" && collided.GetType() == typeof(BoxCollider2D))
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

            if(enemy.getHealth() <= 0)
                incrementCoins();
        }



        //Wall collision
        else if ((collided.gameObject.tag.Equals("Wall") || collided.gameObject.tag.Equals("Platform")) && bodyCollider.IsTouching(collided))
        {
            touchingWall = true;
            if (collided.transform.position.x < this.transform.position.x)
                wallIsLeft = true;
            else
                wallIsLeft = false;

            Debug.Log("Is wall left?   " + wallIsLeft);
        }

        //Taking damage from an enemy
        takeDamageFromEnemy(collided);

    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        Collider2D collided = collision.collider;

        if (footCollider.IsTouching(collided) && collided.gameObject.tag == "Platform")
        {
            onGround = true;
            animator.SetBool("onGround", onGround);
            Debug.Log("On the ground");
        }

        takeDamageFromEnemy(collided);
    }

    private void OnCollisionExit2D(Collision2D collision)
    {

        Collider2D collided = collision.collider;

        //Ground + Foot collision
        if (onGround && collided.gameObject.tag == "Platform")
        {
            onGround = false;
            Debug.Log("Off the ground");
        }
        //else if (onPlat && collided.gameObject.tag == "Platform")
        //{
        //    onPlat = false;
        //    Debug.Log("Off the platform");
        //}

        // animator.SetBool("onGround", onGround || onPlat);
        animator.SetBool("onGround", onGround);

        if (collided.gameObject.tag.Equals("Wall") || collided.gameObject.tag.Equals("Platform"))
            touchingWall = false;
    }

    private void OnTriggerEnter2D(Collider2D collided)
    {
        if (collided.tag == "Coin" && bodyCollider.IsTouching(collided))
        {

            incrementCoins();
            collided.gameObject.SetActive(false);
        }
    }

    private void takeDamageFromEnemy(Collider2D collided)
    {
        int knockX = 7;
        int knockY = 8;

        if ((bodyCollider.IsTouching(collided) || headCollider.IsTouching(collided)) && collided.gameObject.tag == "Enemy" && iFrames == 0)
        {
            Enemy enemy = collided.gameObject.GetComponent<Enemy>();

            if (collided.transform.position.x < this.transform.position.x)
                rb.velocity = new Vector2(knockX, knockY);
            else
                rb.velocity = new Vector2(-knockX, knockY);

            stunTimer = 150;
            iFrames = 780;

            health -= enemy.getAtk();
            damageText.showDamage(bodyCollider.transform.position, atk, 'r');

            Debug.Log("OUCH!");

            if (isDead())
            {
                this.gameObject.layer = LayerMask.NameToLayer("DeadMario");
                animator.SetBool("isDead", true);
                deadTimer = 4 * 60;
                rb.velocity = new Vector2(0, 11f);
                stunTimer = 0;
                animator.SetInteger("stunTime", stunTimer);
                iFrames = 0;
            }

        }
    }

    private void horizontalMovement()
    {
        //Moving left and right
        moveDirection = playerMovement.ReadValue<Vector2>();

        if (touchingWall && ((moveDirection.x < 0 && wallIsLeft) || (moveDirection.x > 0 && !wallIsLeft)))
            moveDirection.x = 0;

        if (!isCrouching)
            rb.velocity = new Vector2(moveDirection.x * moveSpeed, rb.velocity.y);

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
        // if (Input.GetKeyDown(KeyCode.Space) && (onGround || onPlat))
        if (Input.GetKeyDown(KeyCode.Space) && onGround)
            rb.velocity = new Vector2(moveDirection.x * moveSpeed, jumpSpeed);
        else if (Input.GetKeyUp(KeyCode.Space) && rb.velocity.y > 0)
            rb.velocity = new Vector2(moveDirection.x * moveSpeed, 1);

        animator.SetFloat("ySpeed", rb.velocity.y);

    }

    private void crouch()
    {
        //Crouching
        // if (Input.GetKey(KeyCode.S) && (onGround || onPlat))
        if (Input.GetKey(KeyCode.S) && onGround)
        {
            if (!isCrouching)
            {
                bodyCollider.size = new Vector2(xStandBox, yCrouchBox);
                bodyCollider.offset = new Vector2(xStandOff, yCrouchOff);
                footCollider.offset = new Vector2(xStandOff, yFootCrouchOff);

                headCollider.offset = new Vector2(xStandOff, yHeadCrouchOff);

                rb.position = new Vector2(rb.position.x, rb.position.y + (yFootStandOff - yFootCrouchOff + footCollider.size.y / 2));
                rb.velocity = new Vector2(0, 0);
            }
            isCrouching = true;
        }
        else
        {
            //if (Physics2D.Raycast(transfrm.position, Vector2.up, 5)) {

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
            //}

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
    public int getMaxHealth()
    {
        return maxHealth;
    }
    public void setMaxHealth(int max)
    {
        maxHealth = max;
    }

    public bool isDead()
    {
        return getHealth() <= 0;
    }

    public int getNumCoins()
    {
        return numCoins;
    }
    public void incrementCoins()
    {
        numCoins++;
        if(numCoins % 100 == 0)
        {
            setMaxHealth(getMaxHealth() + 5);
            setHealth(getHealth() + 5);
        }
    }
}