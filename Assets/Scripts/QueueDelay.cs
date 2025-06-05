
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.Linq;
using UnityEngine.InputSystem;

public class QueueDelay : MonoBehaviour
{
    public bool _inQueue = true;
    public static bool IsStarted = false;

    [SerializeField] private List<GameObject> _gameUI;

    [SerializeField] public GameObject[] Players;

    [SerializeField] private float _delay;
    [SerializeField] private int _maxDelay;

    [SerializeField] private TMP_Text _timerText;

    [SerializeField] private GameObject _playerInputManager;

    [SerializeField] private GameObject[] _initialToppingSpawns;
    private GameObject[] _toppings;
    [SerializeField] private GameObject _toppingPrefab;
    //[SerializeField] private GameObject _InstructionPrefab;
    //private GameObject _instructions;
    //private int _currentPlayers;

    [SerializeField] private List<GameObject> _currentPlayers = new List<GameObject>();
    [SerializeField] private List<GameObject> _currentToppings = new List<GameObject>();
    [SerializeField] List<GameObject> _readyPlayer;


    [Header("intro")]
    [SerializeField] private List<GameObject> _explinaition;
    [SerializeField] private int _explinaitionCount = 0;

    [SerializeField] private float _explinaitionDelay = 1;
    private float _Explenationtimer;

    private PlayerInput _input;
    [SerializeField] private GameObject _playerInput;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //_initialToppingSpawns = GameObject.FindGameObjectsWithTag("InitialToppingSpawn");
        _Explenationtimer = _explinaitionDelay;

        _input = GetComponent<PlayerInput>();

        //_instructions = Instantiate(_InstructionPrefab, transform.position, Quaternion.identity);

        Mathf.Clamp(_delay, 0, _maxDelay);

        foreach (GameObject game in _gameUI)
        {
            game.SetActive(false);
            Debug.Log(game.name);
        }

    }

    // Update is called once per frame
    void Update()
    {
        GetPlayers();
        SpawnInitialToppings(_toppingPrefab, _initialToppingSpawns, Players);


        Instruction(_explinaitionCount);

        QueTimer();
        StartGame();
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
            _timerText.text = "<br>Waiting for Players" + "<br>To Start throw your topping on the pizza";
            _delay = _maxDelay;
        }
        else if (_readyPlayer.Count == _currentPlayers.Count)
        {
            _timerText.enabled = true;

            _delay -= Time.deltaTime;

            _timerText.text = "Starting in: " + _delay.ToString("F0");
        }

        /*if (Players.Count() > 1)
        {
            if (_delay <= 0)
            {
                _inQueue = false;
            }
        }*/


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

            foreach (GameObject topping in _readyPlayer)
            {
                Destroy(topping);
            }

            foreach (GameObject player in _currentPlayers)
            {
                player.GetComponent<PickupHandler>()._canPickUpOnlyIDToppings = false;
            }


            foreach (GameObject game in _gameUI)
            {
                game.SetActive(true);
            }
            //Destroy(_instructions);
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
                player.GetComponent<PickupHandler>()._canPickUpOnlyIDToppings = true;
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

                topping.GetComponent<ToppingHandler>().ChangeIndex(_currentPlayers.Count - 1);

                _currentToppings.Add(topping);
            }
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        //other = gameObject.GetComponentInChildren<Collider>();
        //Debug.Log("test");
        if (other.tag == "Ingredient")
        {
            other.gameObject.GetComponent<ToppingHandler>().IsScored = true;
            _readyPlayer.Add(other.gameObject);
        }
    }

    /*private void OnCollisionStay(Collision collision)
    {
        Debug.Log(collision.gameObject.name);
    }*/

    private void Instruction(int i)
    {




        if (i < _explinaition.Count)
        {
            _Explenationtimer -= Time.deltaTime;
            _explinaition[i].SetActive(true);

        }


        if (_input.actions["Submit"].triggered && _Explenationtimer <= 0)
        {
            _explinaition[i].SetActive(false);



            _explinaitionCount++;
            _Explenationtimer = _explinaitionDelay;
        }

        if (i == _explinaition.Count)
        {
            _input.enabled = false;
            _explinaition[i - 1].SetActive(false);
            _playerInput.SetActive(true);
            
        }
    }

}



