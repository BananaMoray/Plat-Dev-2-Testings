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
    [SerializeField] private AudioClip _winClip;
    [SerializeField] private AudioSource _coundDownSrc;

    bool _hasPlayed = false;

    //private bool _hasPlayedCountdown = false;

    private int _lastSecondPlayed = -1; // Initialize to -1 so 10 is triggered first


    void Start()
    {
        _timeRemainingText.enabled = false;
        _gameManager = _gameManagerObject.GetComponent<GameManager>();

        _coundDownSrc.clip = _coundDownClip;
        _coundDownSrc.pitch = 0.2f;
    }

    // Update is called once per frame
    void Update()
    {


        int currentSecond = Mathf.FloorToInt(_gameManager._timer);

        if (currentSecond <= 10 && currentSecond >= 0 && currentSecond != _lastSecondPlayed)
        {
            _timeRemainingText.text = currentSecond.ToString();
            _timeRemainingText.enabled = true;

            _coundDownSrc.pitch = _coundDownSrc.pitch + 0.01f;
            Debug.Log(_coundDownSrc.pitch);
            _coundDownSrc.Play();
            _lastSecondPlayed = currentSecond;
        }
        if (currentSecond <=0&&!_hasPlayed)
        {
            _coundDownSrc.clip = _winClip;
            if (!_coundDownSrc.isPlaying)
            {
                _coundDownSrc.pitch = 1;
                _coundDownSrc.Play();
                Debug.Log("has played");
            }
            _hasPlayed = true;
        }


    }
}
