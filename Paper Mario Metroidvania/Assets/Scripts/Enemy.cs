using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Enemy : MonoBehaviour
{
    public Rigidbody2D rb;
    public Animator animator;

    public BoxCollider2D headCollider;
    public CapsuleCollider2D bodyCollider;
    public BoxCollider2D footCollider;

    public GameObject bouncingCoin;
    private GameObject coin = null;

    Vector2 moveDirection = Vector2.zero;
    Vector2 facingLeft;
    Vector2 facingRight;

    bool isFacingLeft = true;

    public float xSpeed;
    int xDirection;
    int health = 2;
    int atk = 1;
    int damageTimer = 0;
    int waitBeforeTurn = 0;


    // Start is called before the first frame update
    void Start()
    {
        facingLeft = new Vector2(transform.localScale.x, transform.localScale.y);
        facingRight = new Vector2(-transform.localScale.x, transform.localScale.y);

        //this.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

        if (isFacingLeft)
        {
            xDirection = -1;
            transform.localScale = facingLeft;
        }
        else
        {
            xDirection = 1;
            transform.localScale = facingRight;
        }

        if (damageTimer == 0)
            rb.velocity = new Vector2(xSpeed * xDirection, rb.velocity.y);
        else
            rb.velocity = new Vector2(0, rb.velocity.y);

        if (damageTimer > 0)
            damageTimer--;

        animator.SetInteger("damaged", damageTimer);

        if (health <= 0 && damageTimer <= 0)
        {
            this.gameObject.SetActive(false);
            coin.SetActive(false);
        }
            
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Collider2D collided = collision.collider;

        if (bodyCollider.IsTouching(collided) && collided.gameObject.tag != "Player" && shouldTurnAround(collided))
            isFacingLeft = !isFacingLeft;
        else if(bodyCollider.IsTouching(collided) && collided.gameObject.tag == "Player" && shouldTurnAround(collided))
            waitBeforeTurn = 7;
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        Collider2D collided = collision.collider;
        if (bodyCollider.IsTouching(collided) && collided.gameObject.tag == "Player") {
            if(waitBeforeTurn != 0) {
                waitBeforeTurn--;
            }
            else if(shouldTurnAround(collided)) {
                isFacingLeft = !isFacingLeft;
            }
            
        }
        else if(bodyCollider.IsTouching(collided) && shouldTurnAround(collided)) {
            isFacingLeft = !isFacingLeft;
        }
            
        //else if((waitBeforeTurn == 0 && collided.gameObject.tag == "Player" && shouldTurnAround(collided) && collided.gameObject.GetComponent<PlayerController>().footCollider != collided) || shouldTurnAround(collided))
            
    }

    private bool shouldTurnAround(Collider2D collided)
    {
        return ((collided.gameObject.transform.position.x <= rb.position.x && isFacingLeft) || (collided.gameObject.transform.position.x >= rb.position.x && !isFacingLeft)) && !footCollider.IsTouching(collided) && waitBeforeTurn == 0;
    }
    private bool isPlayerOnGround(Collider2D collided)
    {
        return collided.gameObject.tag == "Player" && collided.gameObject.GetComponent<PlayerController>().onGround;
    }

    public int getHealth()
    {
        return health;
    }
    public void setHealth(int num)
    {
        health = num;
    }
    public void takeDamage(int num)
    {
        health -= num;
        if (health <= 0) {
            damageTimer = 150;
            this.gameObject.layer = LayerMask.NameToLayer("DeadEnemies");
            coin = Instantiate(bouncingCoin, new Vector3(this.gameObject.transform.position.x, this.gameObject.transform.position.y + 1, this.gameObject.transform.position.z), Quaternion.identity);
            coin.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 7);
        }
        else
            damageTimer = 90;
        
    }


    public int getAtk()
    {
        return atk;
    }
    public void setAtk(int num)
    {
        atk = num;
    }

}
