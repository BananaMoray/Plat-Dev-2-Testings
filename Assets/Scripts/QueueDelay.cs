
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.Linq;

public class QueueDelay : MonoBehaviour
{
    [SerializeField] private bool _inQueue = false;
    public static bool IsStarted = false;

    [SerializeField] private List<GameObject> _gameUI;

    [SerializeField] public GameObject[] Players;

    [SerializeField] private float _delay;
    [SerializeField] private int _maxDelay;

    [SerializeField] private TMP_Text _timerText;

    [SerializeField] private GameObject _playerInputManager;

    private GameObject[] _initialToppingSpawns;
    private GameObject[] _toppings;
    [SerializeField] private GameObject _toppingPrefab;
    //private int _currentPlayers;

    [SerializeField] private List<GameObject> _currentPlayers = new List<GameObject>();
    [SerializeField] private List<GameObject> _currentToppings = new List<GameObject>();


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        _initialToppingSpawns = GameObject.FindGameObjectsWithTag("InitialToppingSpawn");

        Mathf.Clamp(_delay, 0, _maxDelay);

        foreach (GameObject game in _gameUI)
        {
            game.SetActive(false);
        }

    }

    // Update is called once per frame
    void Update()
    {
        GetPlayers();
        SpawnInitialToppings(_toppingPrefab, _initialToppingSpawns, Players);

        //QueTimer();
        //StartGame();
    }


    void GetPlayers()
    {
        Players = GameObject.FindGameObjectsWithTag("Player");
        _toppings = GameObject.FindGameObjectsWithTag("Ingredient");
    }

    void QueTimer()
    {
        if (Players.Count() < 2)
        {
            _timerText.text = "Waiting for Players";
            _delay = _maxDelay;
        }
        else
        {
            _timerText.enabled = true;
            if (_delay > 0)
            {
                _delay -= Time.deltaTime;
            }

            _timerText.text = "Starting in: " + _delay.ToString("F0");
        }

        if (Players.Count() > 1)
        {
            if (_delay <= 0)
            {
                _inQueue = false;
            }
        }


    }

    void StartGame()
    {
        if (_delay <= 0)
        {
            IsStarted = true;
        }

        if (IsStarted)
        {

            for (int i = 0; i < Players.Count(); i++)
            {
                PizzaScoreZone.PlayerScores[i] = 0;
            }

            foreach (GameObject game in _gameUI)
            {
                game.SetActive(true);
            }
            gameObject.GetComponent<QueueDelay>().enabled = false;
            _playerInputManager.SetActive(false);
        }
    }


    void SpawnInitialToppings(GameObject prefab, GameObject[] toppicSpawns, GameObject[] players)
    {
        GameObject[] currenttoppings = new GameObject[4];



        foreach (GameObject player in players)
        {
            //currentPlayers.Add(player);
            if (!_currentPlayers.Contains(player))
            {
                int i = player.GetComponent<CharacterManager>().PlayerIndex;
                currenttoppings[i] = Instantiate(prefab, toppicSpawns[i].transform.position, Quaternion.identity);
                //prefab.GetComponent<ToppingHandler>().PlayerIndex = i;

                _currentPlayers.Add(player);
            }
        }

        foreach (GameObject topping in _toppings)
        {
            if (!_currentToppings.Contains(topping))
            {
                topping.GetComponent<ToppingHandler>().PlayerIndex = _currentPlayers.Count - 1;

                _currentToppings.Add(topping);
            }
        }
    }









}
