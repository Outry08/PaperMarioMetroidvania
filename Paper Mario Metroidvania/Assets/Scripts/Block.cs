using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{

    public Animator animator;
    public PlayerController player;

    public bool hasThing;
    public GameObject contents = null;
    private List<GameObject> item = new List<GameObject>();
    public bool breakable;  //Able to be broken by hitting it with head
    public bool destroyable; //Able to be broken by something like a bomb
    public int numCoins = 0;
    bool breaking = false;
    bool coinBouncing = false;
    int breakCount = 0;

    private void Start()
    {
        if((contents == null && hasThing) || (!hasThing && contents != null))
            Debug.Log("Block Error: contents ≠ hasThing");
        if(hasThing && breakable)
            Debug.Log("Block Error: hasThing = breakable");
        if ((numCoins > 0 && (contents == null || contents.tag != "Coin")) || ((contents != null && contents.tag == "Coin") && numCoins <= 0))
            Debug.Log("Block Error: numCoins ≠ contents");
        if (player == null)
            Debug.Log("Block Error: Missing player");
    }
    private void Update()
    {
        if (breaking) {
            breakCount--;
            if (breakCount == 0)
                this.gameObject.SetActive(false);
            else if(breakCount == 118)
            {
                this.gameObject.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                this.gameObject.GetComponent<BoxCollider2D>().size = new Vector2(2, 2);
            }
                
        }

        if (coinBouncing)
        {
            //Debug.Log("BOUNCING");
            if (item[0].transform.position.y <= this.gameObject.transform.position.y)
            {
                item[0].SetActive(false);
                item.RemoveAt(0);
                item.TrimExcess();
                if (item.Count <= 0)
                    coinBouncing = false;
                player.incrementCoins();
            }
        }
                
            
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Collider2D collided = collision.collider;

        //Hit from below by Mario
        if(player.headCollider.IsTouching(this.gameObject.GetComponent<BoxCollider2D>()))
        {
            if(breakable)
                breakBlock();
            else if(hasThing)
                showThing();
        }
    }

    void showThing()
    {
        
        item.Add(Instantiate(contents, new Vector3(this.gameObject.transform.position.x, this.gameObject.transform.position.y + 1, this.gameObject.transform.position.z), Quaternion.identity));
        if(item[item.Count - 1].tag == "Coin")
        {
            item[item.Count - 1].GetComponent<Rigidbody2D>().velocity = new Vector2(0, 7);
            coinBouncing = true;
            numCoins--;
        }

        if (numCoins <= 0)
        {
            animator.SetBool("Empty", true);
            hasThing = false;
            this.gameObject.transform.localScale = new Vector3(0.057f, 0.057f, 0.057f);
            this.gameObject.GetComponent<BoxCollider2D>().size = new Vector2(17.53f, 17.53f);
        }
    }

    void breakBlock()
    {
        animator.SetBool("Broken", true);
        breaking = true;
        breakCount = 120;
        breakable = false;
        destroyable = false;
        this.gameObject.GetComponent<BoxCollider2D>().isTrigger = true;
    }
}
