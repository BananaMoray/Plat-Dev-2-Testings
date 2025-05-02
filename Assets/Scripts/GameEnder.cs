using UnityEngine;

public class GameEnder : MonoBehaviour
{
    [SerializeField]
    private GameObject _gameEnderObject;
   
    private GameManager _gameManager;

    private bool _isGameEnded;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _gameManager = _gameEnderObject.GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        //pause game when ended
        //redirect to the vizualize winner

        if (_gameManager._timer <= 0)
        {
            _isGameEnded = true;
            Debug.Log("ended");
            Time.timeScale = 0;


        }
    }
}
