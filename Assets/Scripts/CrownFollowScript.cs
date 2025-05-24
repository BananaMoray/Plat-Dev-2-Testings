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
        //Debug.Log("Winning Player Index: " + _winningPlayerIndex);

        //Debug.Log("List of previous Scores: " + string.Join(", ", _listOfPreviousScores));
        //Debug.Log("List of Scores: " + string.Join(", ", _listOfScores));
        //Debug.Log("best score: " + _listOfScores.Max());
        //Debug.Log("best score: " + _listOfPreviousScores.Max());


        for (int i = 0; i < _queueDelay.Players.Count(); i++)
        {
            _listOfScores[i] = PizzaScoreZone.GetPlayerScore(i);
            _playerPositions[i] = _queueDelay.Players[i].transform.position;
        }
        //if (_listOfScores.Max() > _listOfPreviousScores.Max())
        //{
            for (int i = 0; i < _queueDelay.Players.Count(); i++)
            {
                if (_listOfScores[i] == _listOfScores.Max())
                {
                    _winningPlayerIndex = i;
                }
            }
        //}
        _listOfPreviousScores = _listOfScores;
        //if (active == false)
        //{

        //    active = true;
        //}
        transform.position = Vector3.Lerp(transform.position, _playerPositions[_winningPlayerIndex] + new Vector3(0, 3, 0), Time.deltaTime * 5);

    }
}
