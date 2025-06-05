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



    bool _hasPlayed = false;

    [SerializeField]
    private GameObject _timeUI;

    private int _lastSecondPlayed = -1; 


    void Start()
    {
        _timeRemainingText.enabled = false;
        _gameManager = _gameManagerObject.GetComponent<GameManager>();

        //_coundDownSrc.clip = _coundDownClip;
        //_coundDownSrc.pitch = 0.2f;
    }

    // Update is called once per frame
    void Update()
    {


        int currentSecond = Mathf.FloorToInt(_gameManager._timer);

        if (currentSecond <= 10 && currentSecond >= 0 && currentSecond != _lastSecondPlayed)
        {
            

            _timeRemainingText.text = currentSecond.ToString();
            _timeRemainingText.enabled = true;
            _lastSecondPlayed = currentSecond;
            _timeUI.SetActive(false);
        }



    }
}
