using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextTracker : MonoBehaviour
{

    public PlayerController player;

    public Text healthText;
    private int currentHealth, maxHealth;

    public Text numberText;
    public SpriteRenderer damageDealStar;
    public SpriteRenderer damageTakeStar;
    public SpriteRenderer healthGainHeart;

    public Transform eventTextCanvas;

    private List<SpriteRenderer> activeSprites = new List<SpriteRenderer>();
    private List<Text> activeEventTexts = new List<Text>();
    private List<int> activeEventTimers = new List<int>();

    public Text coinText;
    private int currentCoins, maxCoins;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        //Health text UI
        currentHealth = player.getHealth();
        maxHealth = player.getMaxHealth();
        updateHealthText();

        //Coin text UI
        currentCoins = player.getNumCoins();
        maxCoins = player.getNextLevel();
        updateCoinText();


        //Damage Text
        for(int i = 0; i < activeSprites.Count; i++)
        {

            Debug.Log(activeSprites.Count);
            if (activeEventTimers[i] > 0)
                activeEventTimers[i]--;

            if (activeEventTimers[i] == 0)
            {
                activeSprites[i].gameObject.SetActive(false);
                activeEventTexts[i].gameObject.SetActive(false);
                

                activeSprites.RemoveAt(i);
                activeEventTexts.RemoveAt(i);
                activeEventTimers.RemoveAt(i);
            }
            else if (activeSprites[i].gameObject.tag == "Heart")
            {
                activeSprites[i].transform.position = new Vector2(player.headCollider.transform.position.x, player.headCollider.transform.position.y + 1.5f);
                activeEventTexts[i].transform.position = new Vector2(player.headCollider.transform.position.x, player.headCollider.transform.position.y + 1.5f);
            }

        }

    }

    void updateHealthText()
    {
        healthText.text = "Health: " + currentHealth + "/" + maxHealth;
    }
    void updateCoinText()
    {
        coinText.text = "Coins: " + currentCoins + "/" + maxCoins;
    }

    public void showDamage(Vector2 position, int atk, char colour)
    {
        SpriteRenderer star;
        Text currentText;
        Color orange = new Color(1.0f, 0.64f, 0.0f);

        currentText = Instantiate<Text>(numberText, eventTextCanvas);
        currentText.transform.position = position;
        currentText.text = atk.ToString();
        currentText.color = orange;
        activeEventTexts.Add(currentText);
        
        if (colour == 'y') 
            star = Instantiate(damageDealStar);
        else 
            star = Instantiate(damageTakeStar);

        activeSprites.Add(star);

        star.transform.position = position;

        currentText.gameObject.SetActive(true);
        star.gameObject.SetActive(true);
        activeEventTimers.Add(120);
    }

    public void showHealth(Vector2 position, int amnt)
    {
        SpriteRenderer heart;
        Text currentText;

        currentText = Instantiate<Text>(numberText, eventTextCanvas);
        currentText.transform.position = position;
        currentText.text = amnt.ToString();
        currentText.color = Color.red;
        activeEventTexts.Add(currentText);

        heart = Instantiate(healthGainHeart);

        activeSprites.Add(heart);

        heart.transform.position = position;

        currentText.gameObject.SetActive(true);
        heart.gameObject.SetActive(true);
        activeEventTimers.Add(480);
    }
}
