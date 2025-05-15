
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.Linq;

public class QueueDelay : MonoBehaviour
{
    [SerializeField] private bool _inQueue = false;
    [SerializeField] public bool _startGame = false;

    [SerializeField] private List<GameObject> _gameUI;

    [SerializeField] public GameObject[] Players;

    [SerializeField] private float _delay;
    [SerializeField] private int _maxDelay;

    [SerializeField] private TMP_Text _timerText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

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
        QueTimer();
        StartGame();
    }


    void GetPlayers()
    {
        Players = GameObject.FindGameObjectsWithTag("Player");
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

            _timerText.text ="Starting in: " + _delay.ToString("F0");
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
        if(_delay <= 0)
        {
            _startGame = true;
        }

        if(_startGame)
        {
            
            for (int i = 0;  i < Players.Count(); i++)
            {
                PizzaScoreZone.PlayerScores[i] = 0;
            }

            foreach (GameObject game in _gameUI)
            {
                game.SetActive(true);
            }
            gameObject.GetComponent<QueueDelay>().enabled = false;
        }
    }


}
