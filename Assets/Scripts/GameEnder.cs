using UnityEngine;
using UnityEngine.TextCore.Text;

public class GameEnder : MonoBehaviour
{
    [SerializeField]
    private GameObject _gameManagerObject;

    [SerializeField]
    private GameObject _endScreenUI;

    [SerializeField]
    private GameObject _characters;

    private GameManager _gameManager;

    private bool _isGameEnded;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _endScreenUI.SetActive(false);
        _gameManager = _gameManagerObject.GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        //pause game when ended
        //redirect to the vizualize winner
         
        if (_gameManager._timer <= 0)
        {
            _isGameEnded = true;
            Time.timeScale = 0;
        }
        if (_isGameEnded)
        {
            //_mainCamera.enabled = false;
            //_endScreenCamera.enabled = true;
        }
    }
}
