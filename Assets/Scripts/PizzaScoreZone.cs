using System.Collections.Generic;
using UnityEngine;

public class PizzaScoreZone : MonoBehaviour
{
    [SerializeField]
    private GameManager _gameManager;

    private void OnTriggerEnter(Collider collider)
    {
        //this makes sure that the object that entered the scene is actually a topping
        //i should probably return to CharacterControl and do the same there -- Freya
        ToppingHandler topping = collider.GetComponent<ToppingHandler>();

        //if the object doesnt possess a toppinghandler, nothing happens
        if (topping != null)
        {
            int playerIndex = topping.PlayerIndex;

            if (_gameManager.IsValidPlayer(playerIndex) && !topping.IsScored)
            {
                _gameManager.PlayerScores[playerIndex]++;
                topping.IsScored = true;

                Debug.Log($"Player {playerIndex} Score: " + _gameManager.GetPlayerScore(playerIndex));
            }
        }
    }

    //inversion of the previous code, now we can also remove toppings
    private void OnTriggerExit(Collider collider)
    {
        ToppingHandler topping = collider.GetComponent<ToppingHandler>();

        //if (!topping.CanBePickedUp) return;

        if (topping != null && topping.IsScored)
        {
            int playerIndex = topping.PlayerIndex;

            if (_gameManager.IsValidPlayer(playerIndex))
            {
                _gameManager.PlayerScores[playerIndex]--;
                topping.IsScored = false;
                topping.CanBePickedUp = true;

                Debug.Log($"Player {playerIndex} Score: " + _gameManager.GetPlayerScore(playerIndex));
            }
        }
    }


}
