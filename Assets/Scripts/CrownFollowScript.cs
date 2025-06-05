using System.Linq;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;

public class CrownFollowScript : MonoBehaviour
{
    private int[] _listOfScores = new int[4];
    private int[] _listOfPreviousScores = new int[4];

    private Vector3[] _playerPositions = new Vector3[4];

    [SerializeField]
    private QueueDelay _queueDelay;

    private int _winningPlayerIndex = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    private bool active;
    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < _queueDelay.Players.Count(); i++)
        {
            _listOfScores[i] = PizzaScoreZone.GetPlayerScore(i);
            _playerPositions[i] = _queueDelay.Players[i].transform.position;
        }
            for (int i = 0; i < _queueDelay.Players.Count(); i++)
            {
                if (_listOfScores[i] == _listOfScores.Max())
                {
                    _winningPlayerIndex = i;
                }
            }
        _listOfPreviousScores = _listOfScores;
        transform.position = Vector3.Lerp(transform.position, _playerPositions[_winningPlayerIndex] + new Vector3(0, 3.5f, 0), Time.deltaTime * 5);
    }
}
