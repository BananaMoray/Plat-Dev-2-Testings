using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using System.Linq;


public class RespawnHandler : MonoBehaviour
{


    //private Rectangle _bounds = new Rectangle(0, 0, 100, 50);

    [SerializeField]
    GameObject[] _players;
    [SerializeField]
    GameObject[] _toppings;

    [SerializeField]
    Transform[] _respawns;

    [SerializeField] private QueDelay _delay;
    [SerializeField] private bool _start;


    [SerializeField] private int _maxWeight = 15;

    [SerializeField] private GameObject _toppingPrefab;
    [SerializeField] private float _radius = 5f;
    [SerializeField] private float _spawnDelay = 20;
    [SerializeField] private float _toppingSpawnTimer;
    [SerializeField] private int _maxTime;
    [SerializeField] private int _height = 10;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        _players = GameObject.FindGameObjectsWithTag("Player");
        _toppings = GameObject.FindGameObjectsWithTag("Ingredient");

        foreach (GameObject player in _players)
        {
            HitBoundary(player);
        }
        /*foreach (GameObject topp in _toppings)
        {
            HitBoundary(topp);
        }*/

        _start = _delay._startGame;

        if (_start)
        {
            _spawnDelay -= Time.deltaTime;
            if (_spawnDelay <= 0)
            {
                SpawnAtRandomCirclePosition();
            }
        }

    }

    void HitBoundary(GameObject obj)
    {

        if (obj.transform.position.y <= -25)
        {
            print("below -25");

            if (obj.transform.position.x <= 0)
            {
                //Left Side
                if (obj.transform.position.z <= 0)
                {
                    //Bottom Left
                    Respawn(obj, 0);
                    print("BL");
                }
                if (obj.transform.position.z >= 0)
                {
                    //Top Left
                    Respawn(obj, 1);
                    print("TL");

                }

            }
            if (obj.transform.position.x >= 0)
            {
                //Right Side
                if (obj.transform.position.z <= 0)
                {
                    //Bottom Right
                    Respawn(obj, 2);
                    print("BR");

                }
                if (obj.transform.position.z >= 0)
                {
                    //Top Right
                    Respawn(obj, 3);
                    print("TR");

                }

            }

            obj.transform.position = new Vector3(0, 5, 0);
        }

    }

    void Respawn(GameObject obj, int i)
    {
        print("Respawn triggered");

        CharacterController _char = obj.GetComponent<CharacterController>();

        _char.enabled = false;

        obj.transform.position = _respawns[i].position;

        _char.enabled = true;

        _char.transform.SetParent(null);
        _char.transform.rotation = Quaternion.identity;
        _char.GetComponent<CharacterController>().enabled = true;
        _char.GetComponent<CharacterControl>().IsHit = false;

        print(obj.name);

    }

    void SpawnAtRandomCirclePosition()
    {
        _toppingSpawnTimer -= Time.deltaTime;

        float angle = Random.Range(0f, Mathf.PI * 2f);
        Vector3 spawnPosition = new Vector3(Mathf.Cos(angle) * _radius, _height, Mathf.Sin(angle) * _radius);

        if (_toppingSpawnTimer <= 0)
        {
            int selectedPlayerIndex = GetPlayerByWeight();

            GameObject topping = Instantiate(_toppingPrefab, spawnPosition, Quaternion.identity);
            ToppingHandler toppingHandler = topping.GetComponent<ToppingHandler>();

            if (toppingHandler != null)
            {
                toppingHandler.PlayerIndex = selectedPlayerIndex;
            }

            _toppingSpawnTimer = _maxTime;
        }


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
            int weight = Mathf.Max(1, _maxWeight - toppingCounts[i]);


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
