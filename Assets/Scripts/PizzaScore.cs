using System.Collections.Generic;
using UnityEngine;

public class PizzaScoreZone : MonoBehaviour
{
    [SerializeField] 
    private int _maxPlayers = 4;
    private List<int> _playerScores;

    //[SerializeField]
    //private GameObject _pizza;
    //[SerializeField]
    //private MeshCollider _meshCollider;

    private void Awake()
    {

        _playerScores = new List<int>(new int[_maxPlayers]);
    }

    private void OnTriggerEnter(Collider collider)
    {
        //this makes sure that the object that entered the scene is actually a topping
        //i should probably return to CharacterControl and do the same there -- Freya
        ToppingHandler topping = collider.GetComponent<ToppingHandler>();

        //if the object doesnt possess a toppinghandler, nothing happens
        if (topping != null)
        {
            int playerIndex = topping.PlayerIndex;

            if (IsValidPlayer(playerIndex) && !topping.IsScored)
            {
                _playerScores[playerIndex]++;
                topping.IsScored = true;

                Debug.Log($"Player {playerIndex} Score: " + GetPlayerScore(playerIndex));
            }
        }
    }

    //inversion of the previous code, now we can also remove toppings
    private void OnTriggerExit(Collider collider)
    {
        ToppingHandler topping = collider.GetComponent<ToppingHandler>();

        if (topping != null && topping.IsScored)
        {

            int playerIndex = topping.PlayerIndex;

            if (IsValidPlayer(playerIndex))
            {
                _playerScores[playerIndex]--;
                topping.IsScored = false;

                Debug.Log($"Player {playerIndex} Score: " + GetPlayerScore(playerIndex));
            }
        }
    }

    //return the player scores yay yippie yay
    //it's also public so we can call it in our UI or anywhere else
    public int GetPlayerScore(int playerIndex)
    {
        if (IsValidPlayer(playerIndex))
            return _playerScores[playerIndex];
        return 0;
    }


    //i ran into some issues with exceptions, so now we also check whether or not a players index is correct
    //yes i know, but trust me this will save us so much pain in the future
    private bool IsValidPlayer(int index)
    {
        //first checks if the player index is negative, which should be impossible
        //then checks that it doesn't exceed the playercount altogether
        return index >= 0 && index < _playerScores.Count;
    }
}
