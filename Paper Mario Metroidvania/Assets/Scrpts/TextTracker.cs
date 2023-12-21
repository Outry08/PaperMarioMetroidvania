using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextTracker : MonoBehaviour
{

    public PlayerController player;

    public Text healthText;
    private int prevHealth, currentHealth, maxHealth;

    public Text damageText;
    public SpriteRenderer damageDealStar;
    public SpriteRenderer damageTakeStar;

    public Text coinText;
    private int prevCoins, currentCoins;

    int appearTime = 0;

    // Start is called before the first frame update
    void Start()
    {
        prevHealth = -1;
        maxHealth = player.getMaxHealth();
        prevCoins = -1;
    }

    // Update is called once per frame
    void Update()
    {

        //Health text UI
        currentHealth = player.getHealth();
        if (currentHealth != prevHealth)
            updateHealthText();
        prevHealth = player.getHealth();

        //Coin text UI
        currentCoins = player.getNumCoins();
        if (currentCoins != prevCoins)
            updateCoinText();
        prevCoins = player.getNumCoins();


        //Damage Text
        if (appearTime > 0)
            appearTime--;

        if(appearTime == 0)
        {
            damageText.gameObject.SetActive(false);
            damageDealStar.gameObject.SetActive(false);
            damageTakeStar.gameObject.SetActive(false);
        }

    }

    void updateHealthText()
    {
        healthText.text = "Health: " + currentHealth + "/" + maxHealth;
    }
    void updateCoinText()
    {
        coinText.text = "Coins: " + currentCoins;
    }

    public void showDamage(Vector2 position, int atk, char colour)
    {
        SpriteRenderer star;
        Color orange = new Color(1.0f, 0.64f, 0.0f);
        damageText.transform.position = position;
        damageText.text = atk.ToString();
        damageText.color = orange;
        
        

        if (colour == 'y') {
            star = damageDealStar;
        }
        else {
            star = damageTakeStar;
        }

        star.transform.position = position;

        damageText.gameObject.SetActive(true);
        star.gameObject.SetActive(true);
        appearTime = 120;

    }
}
