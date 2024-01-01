using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mushroom : MonoBehaviour
{

    public Rigidbody2D rb;
    public int moveSpeed = 2;
    int moveDirection = 1;
    bool movingLeft = false;

    // Start is called before the first frame update
    void Start()
    {
        rb.velocity = new Vector2(moveSpeed * moveDirection, 0);

    }

    // Update is called once per frame
    void Update()
    {
        rb.velocity = new Vector2(moveSpeed * moveDirection, rb.velocity.y);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Collider2D collided = collision.collider;

        if (shouldTurnAround(collided))
        {
            moveDirection *= -1;
            movingLeft = !movingLeft;
        }
    }

    private bool shouldTurnAround(Collider2D collided)
    {
        return ((collided.gameObject.transform.position.x <= rb.position.x && movingLeft) || (collided.gameObject.transform.position.x >= rb.position.x && !movingLeft)) && collided.gameObject.tag != "Platform";
    }
}
