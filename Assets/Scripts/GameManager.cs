using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private int _maxPlayers = 4;
    public List<int> PlayerScores;

    [SerializeField]
    TMP_Text _timerVisuals;

    [SerializeField] //in seconds
    private float _timer;

    [SerializeField]
    private float _seconds, _minutes;

    private void Awake()
    {
        PlayerScores = new List<int>(new int[_maxPlayers]);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Timer();
    }

    void Timer()
    {
        _timerVisuals.text = string.Format("{0:00}:{1:00}", _minutes, _seconds);

        if (_timer > 0)
        {
            _seconds = ((int)_timer % 60);
            _minutes = ((int)_timer / 60);
            
            _timer -= Time.deltaTime;
        }
    }

    //return the player scores yay yippie yay
    //it's also public so we can call it in our UI or anywhere else
    public int GetPlayerScore(int playerIndex)
    {
        if (IsValidPlayer(playerIndex))
            return PlayerScores[playerIndex];
        return 0;
    }


    //i ran into some issues with exceptions, so now we also check whether or not a players index is correct
    //yes i know, but trust me this will save us so much pain in the future
    public bool IsValidPlayer(int index)
    {
        //first checks if the player index is negative, which should be impossible
        //then checks that it doesn't exceed the playercount altogether
        return index >= 0 && index < PlayerScores.Count;
    }

}
