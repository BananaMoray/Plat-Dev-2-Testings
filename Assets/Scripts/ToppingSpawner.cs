using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ToppingSpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject[] _players;
    [SerializeField] 
    private QueueDelay _queDelay;
    [SerializeField] 
    private bool _start;


    [SerializeField]
    private GameObject _toppingPrefab;
    [Header("Weight Values")]
    [SerializeField]
    [Tooltip("The starting weight score that the current toppings are being compared to.")]
    private int _maxWeight = 15;
    [SerializeField]
    [Tooltip("Multiplies the topping amount with this value, a higher multiplier leads to faster low weight.")]
    private int _weightMultiplier = 1;

    [Header("Spawn Values")]
    [SerializeField]
    private float _radius = 15f;
    [SerializeField]
    private float _spawnDelay = 0;

    [SerializeField]
    private int _height = 15;
    [SerializeField]
    private int _randomSpawnAmount = 2;

    private float _toppingSpawnTimer;
    private float _spawnTime;
    [SerializeField]
    [Tooltip("The minimum time it takes to spawn a topping.")]
    private float _baseTime = 3f;
    [SerializeField]
    [Tooltip("Increases the base time for toppings to spawn per missing player with this value.")]
    private float _playerTimeWeight = 0.5f;



    private void Update()
    {
        if (QueueDelay.IsStarted)
        {
            if (_players.Length == 0)
            {
                _players = _queDelay.Players;
                _spawnTime = CalculateToppingSpawnTime(_players.Length);
                Debug.Log("It takes " +_spawnTime.ToString() + " seconds to spawn toppings.");
            }

            _spawnDelay -= Time.deltaTime;
            if (_spawnDelay <= 0)
            {
                SpawnAtRandomCirclePosition();
            }
        }
    }

    private float CalculateToppingSpawnTime(int count)
    {
        return _baseTime + (_playerTimeWeight * 4 - (float) count * _playerTimeWeight);
    }

    void SpawnAtRandomCirclePosition()
    {
        int randomInt = Random.Range(1, _randomSpawnAmount);

        _toppingSpawnTimer -= Time.deltaTime;

        if (_toppingSpawnTimer <= 0)
        {
            for (int i = 0; i < randomInt; i++)
            {
                float angle = Random.Range(0f, Mathf.PI * 2f);
                Vector3 spawnPosition = new Vector3(Mathf.Cos(angle) * _radius, _height, Mathf.Sin(angle) * _radius);
                SpawnTopping(spawnPosition);
            }
            _toppingSpawnTimer = _spawnTime;
        }

    }

    private void SpawnTopping(Vector3 spawnPosition)
    {
        int selectedPlayerIndex = GetPlayerByWeight();

        GameObject topping = Instantiate(_toppingPrefab, spawnPosition, Quaternion.Euler(0f, Random.Range(0, 360), 0f));
        ToppingHandler toppingHandler = topping.GetComponent<ToppingHandler>();

        if (toppingHandler != null)
        {
            toppingHandler.PlayerIndex = selectedPlayerIndex;
        }
        Debug.Log("TOpping spawned");
    }

    int GetPlayerByWeight()
    {
        //we are making a list for the topping count per player
        int[] toppingCounts = new int[_players.Count()];

        //we find ALL toppings and slapp them into the topping counter
        //no i dont know why FindObjectByType isnt working
        foreach (ToppingHandler topping in FindObjectsOfType<ToppingHandler>())
        {
            if (topping.PlayerIndex >= 0 && topping.PlayerIndex < _players.Count())
                toppingCounts[topping.PlayerIndex]++;
        }

        List<int> weightList = new List<int>();

        for (int i = 0; i < _players.Count(); i++)
        {
            // we calculate how many toppings the player has
            // a player with 0 toppings has a weight of _maxWeight - 0 to have a topping spawn
            //a player with 5 toppings has a weight of _maxWeight - 5
            //we add a math max so that players with more than _maxWeight toppings dont have a weight of 0 but 1 (there should always be a chance to get toppings)
            int weight = Mathf.Max(1, _maxWeight - toppingCounts[i] * _weightMultiplier);


            //this one is pretty stupid but shoudl still work for now
            //if a player has a weight of 5, they get added 5 timnes to the weightList
            //the weightList will define what topping spawns next
            for (int j = 0; j < weight; j++)
            {
                weightList.Add(i);
            }
        }

        //imagine it like this: weightList[0,0,0,0,0,0,1,1,2,2,2,2,2,3,3]
        return weightList[Random.Range(0, weightList.Count)];
    }
}
