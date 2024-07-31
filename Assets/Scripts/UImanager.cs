using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UImanager : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private Image[] lifeHearts;
    [SerializeField] private Text coinText;
    [SerializeField] private GameObject gameOverPannel;
    [SerializeField] private Text scoreText;
    public static UImanager instance;


    private void Awake()
    {
        instance = this;
    }


    public void UpdateLives(int lives)
    {
        for (int i = 0; i < lifeHearts.Length; i++)
        {
            if (lives > i)
            {
                lifeHearts[i].color = Color.white;
            }
            else
            {
                lifeHearts[i].color = Color.black;
            }
        }
    }


    public void UpdateCoins(int coin)
    {
        coinText.text = coin.ToString();
    }


    public void ShowGameOverPannel()
    {
        gameOverPannel.SetActive(true);
    }


    public void UpdateScore(int score)
    {
        scoreText.text = "Score: " + score + "m";
    }


}
