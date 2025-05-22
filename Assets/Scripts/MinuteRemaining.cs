using TMPro;
using UnityEngine;

public class MinuteRemaining : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
  

    [SerializeField] private TMP_Text _timeRemainingText;

    [SerializeField]
    private GameObject _gameManagerObject;

    private GameManager _gameManager;
    void Start()
    {
        _timeRemainingText.enabled = false;
        _gameManager = _gameManagerObject.GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_gameManager._timer <=10)
        {
            _timeRemainingText.text = "10";
           _timeRemainingText.enabled = true;
        } 
        if (_gameManager._timer <=9)
        {
            _timeRemainingText.text = "9";
           //_timeRemainingText.enabled = true;
        }
    }
}
