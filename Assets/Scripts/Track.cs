using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Track : MonoBehaviour
{
    [SerializeField] private GameObject[] obstacles;
    [SerializeField] private Vector2 numberOfObstacles;
    [SerializeField] private List<GameObject> newObstacles;


    [Header("Coin")]
    [SerializeField] private GameObject coin;
    [SerializeField] private Vector2 numberOfCoins;
    [SerializeField] private List<GameObject> newCoins;


    // Start is called before the first frame update
    void Start()
    {
        int newNumberOfObstacles = (int)Random.Range(numberOfObstacles.x, numberOfObstacles.y);
        for (int i = 0; i < newNumberOfObstacles; i++)
        {
            newObstacles.Add(Instantiate(obstacles[Random.Range(0, obstacles.Length)], transform));
            newObstacles[i].SetActive(false);
        }
        PositionateObstacles();

        // coin
        int newNumberOfCoins = (int)Random.Range(numberOfCoins.x, numberOfCoins.y);
        for (int i = 0; i < newNumberOfCoins; i++)
        {
            newCoins.Add(Instantiate(coin, transform));
            newCoins[i].SetActive(false);
        }
        PositionCoins();
    }


    void PositionateObstacles()
    {
        for (int i = 0; i < newObstacles.Count; i++)
        {
            float posZMin = (297f / newObstacles.Count) + (297f / newObstacles.Count) * i;
            float posZMax = (297f / newObstacles.Count) + (297f / newObstacles.Count) * i + 1;
            Debug.Log("min:  " + posZMin + "max:   " + posZMax);
            newObstacles[i].transform.localPosition = new Vector3(0, 0, Random.Range(posZMin, posZMax));
            newObstacles[i].SetActive(true);
            if (newObstacles[i].GetComponent<ChangeLane>() != null)
            {
                newObstacles[i].GetComponent<ChangeLane>().PositionLane();
            }
        }
    }


    void PositionCoins()
    {
        float minZPos = 10f;
        for (int i = 0; i < newCoins.Count; i++)
        {
            float maxZPos = minZPos + 5f;
            float randomZPos = Random.Range(minZPos, maxZPos);
            newCoins[i].transform.localPosition = new Vector3(transform.position.x, transform.position.y, randomZPos);
            newCoins[i].SetActive(true);
            newCoins[i].GetComponent<ChangeLane>().PositionLane();
            minZPos = randomZPos + 1;
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<Player>().IncreaseSpeed();
            transform.position = new Vector3(0, 0, transform.position.z + 297 * 2);
            PositionateObstacles();
            PositionCoins();
        }

    }
}   
