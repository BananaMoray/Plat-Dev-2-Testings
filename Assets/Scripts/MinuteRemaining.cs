using UnityEngine;

public class MinuteRemaining : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
  

    [SerializeField] private Canvas _timeRemainingCanvas;

    [SerializeField]
    private GameObject _gameManagerObject;

    private GameManager _gameManager;
    void Start()
    {
        _timeRemainingCanvas.enabled = false;
        _gameManager = _gameManagerObject.GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_gameManager._timer <= 60)
        {
           _timeRemainingCanvas.enabled = true;
        }
    }
}
