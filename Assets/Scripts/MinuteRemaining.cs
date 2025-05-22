using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class MinuteRemaining : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created


    [SerializeField] private TMP_Text _timeRemainingText;

    [SerializeField]
    private GameObject _gameManagerObject;
    private GameManager _gameManager;

    [SerializeField] private AudioClip _coundDownClip;
    [SerializeField] private AudioSource _coundDownSrc;

    //private bool _hasPlayedCountdown = false;

    private int _lastSecondPlayed = -1; // Initialize to -1 so 10 is triggered first

    private bool _shouldPlay;
    void Start()
    {
        _timeRemainingText.enabled = false;
        _gameManager = _gameManagerObject.GetComponent<GameManager>();

        _coundDownSrc.clip = _coundDownClip;
    }

    // Update is called once per frame
    void Update()
    {




        //if (_gameManager._timer <= 11 && !_hasPlayedCountdown)
        //{
        //    _timeRemainingText.text = "10";
        //    _timeRemainingText.enabled = true;
        //    _coundDownSrc.Play();
        //    _hasPlayedCountdown = true; // Prevent playing again
        //}
        //if (_gameManager._timer <= 10 && !_hasPlayedCountdown)
        //{
        //    _timeRemainingText.text = "9";
        //    _coundDownSrc.Play();
        //    _hasPlayedCountdown = true; // Prevent playing again
        //}
      
    
        int currentSecond = Mathf.FloorToInt(_gameManager._timer);

        // Only trigger when the second changes and is within 10..0
        if (currentSecond <= 10 && currentSecond >= 0 && currentSecond != _lastSecondPlayed)
        {
            _timeRemainingText.text = currentSecond.ToString();
            _timeRemainingText.enabled = true;
            _coundDownSrc.Play();
            _lastSecondPlayed = currentSecond;
        }
    



    //Debug.Log(_gameManager._timer);


    //if (_gameManager._timer <= 9)
    //{
    //    _timeRemainingText.text = "8";

    //}
    //if (_gameManager._timer <= 8)
    //{
    //    _timeRemainingText.text = "7";

    //}
    //if (_gameManager._timer <= 7)
    //{
    //    _timeRemainingText.text = "6";

    //}
    //if (_gameManager._timer <= 6)
    //{
    //    _timeRemainingText.text = "5";
    //    if (!_coundDownSrc.isPlaying)
    //    {
    //        _coundDownSrc.Play();
    //    }
    //}
    //if (_gameManager._timer <= 5)
    //{
    //    _timeRemainingText.text = "4";

    //}
    //if (_gameManager._timer <= 4)
    //{
    //    _timeRemainingText.text = "3";

    //}
    //if (_gameManager._timer <= 3)
    //{
    //    _timeRemainingText.text = "2";

    //} if (_gameManager._timer <= 2)
    //{
    //    _timeRemainingText.text = "1";

    //}
    //if (_gameManager._timer <= 1)
    //{
    //    _timeRemainingText.text = "0";

    //}
}
}
