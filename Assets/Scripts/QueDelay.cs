
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class QueDelay : MonoBehaviour
{
    [SerializeField] private bool _inQue = false;

    [SerializeField] private List<GameObject> _gameUI;

    [SerializeField] private List<GameObject> _players;

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
    }


    void GetPlayers()
    {
        _players = new List<GameObject>(GameObject.FindGameObjectsWithTag("Player"));
    }

    void QueTimer()
    {
        if (_players.Count < 2)
        {
            _timerText.enabled = false;
            _delay = _maxDelay;
        }
        else
        {
            _timerText.enabled = true;
            if (_delay > 0)
            {
                _delay -= Time.deltaTime;
            }

            _timerText.text = _delay.ToString("F0");
        }

        if (_players.Count > 1)
        {
            if (_delay <= 0)
            {
                _inQue = false;
            }
        }


    }


}
