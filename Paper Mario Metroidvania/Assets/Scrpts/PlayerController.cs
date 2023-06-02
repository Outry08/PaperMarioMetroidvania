using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerController : MonoBehaviour
{
    public Rigidbody2D rb;
    public InputAction playerMovement;
    public InputAction playerJump;
    Vector2 moveDirection = Vector2.zero;
    public float moveSpeed = 5f;
    public float jumpSpeed = 10f;

    private void OnEnable()
    {
        playerMovement.Enable();
        playerJump.Enable();
    }

    private void OnDisable()
    {
        playerMovement.Disable();
        playerJump.Disable();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //Taking the user's key input
        moveDirection = playerMovement.ReadValue<Vector2>();
        rb.velocity = new Vector2(moveDirection.x * moveSpeed, rb.velocity.y);
        //if (playerJump.triggered)
        //{
        //    rb.velocity = new Vector2(moveDirection.x * moveSpeed, jumpSpeed);
        //}
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log(collision.gameObject.name);
    }
}
