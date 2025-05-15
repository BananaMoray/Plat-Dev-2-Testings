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

    // Update is called once per frame
    void Update()
    {
        Debug.Log("Winning Player Index: " + _winningPlayerIndex);
        for (int i = 0; i < _queueDelay.Players.Count(); i++)
        {
            _listOfScores[i] = PizzaScoreZone.GetPlayerScore(i);
            _playerPositions[i] = _queueDelay.Players[i].transform.position;
        }
        _listOfPreviousScores = _listOfScores;
        if (_listOfScores.Max(x => x) > _listOfPreviousScores.Max(x => x))
        {
            for (int i = 0; i < _queueDelay.Players.Count(); i++)
            {
                if (_listOfScores[i] == _listOfScores.Max(x => x))
                {
                    _winningPlayerIndex = i;
                }
            }
        }
        transform.position = Vector3.Lerp(transform.position, _playerPositions[_winningPlayerIndex] + new Vector3(0, 2, 0), Time.deltaTime * 2);

    }
}
