using UnityEngine;
using TMPro;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine.UIElements;


public class GameManager : MonoBehaviour
{

    [SerializeField]
    TMP_Text _timerVisuals;

    [SerializeField] TMP_Text _oneMinuteRemainingText;
    [SerializeField]
    private Vector3 _goToScale;
    [SerializeField]
    private float _scaleSpeed = 0.5f;

    [SerializeField] //in seconds
    public float _timer;

    [SerializeField]
    float _seconds, _minutes;
    [SerializeField]
    private GameObject _fireParticles;


    [SerializeField] private List<GameObject> _UIScreens;
    //numbereing is 0 = start screen, 1 = Pause Screen, 2 = End Screen

    [SerializeField]
    float _durationOf1MinuteRemaining = 0.5f;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        QueueDelay.IsStarted = false;
        _timer *= 60;
        Time.timeScale = 1f;
    }

    // Update is called once per frame
    void Update()
    {
        Timer();
        //IsGamePaused(_UIScreens[1]);
        if (_timer <= 61 && _durationOf1MinuteRemaining > 0)
        {
            _oneMinuteRemainingText.text = "1 MINUTE REMAINING!";
            _fireParticles.SetActive(true);
            _oneMinuteRemainingText.gameObject.transform.localScale = Vector3.Lerp(_oneMinuteRemainingText.gameObject.transform.localScale, _goToScale, _scaleSpeed*Time.deltaTime);
            _durationOf1MinuteRemaining -= Time.deltaTime;
        }
        else
        {
            _oneMinuteRemainingText.gameObject.transform.localScale = Vector3.Lerp(_oneMinuteRemainingText.gameObject.transform.localScale, Vector3.zero, _scaleSpeed * Time.deltaTime);
        }
    }

    void Timer()
    {
        _timerVisuals.text = string.Format("{0:00}:{1:00}", _minutes, _seconds);

        if (_timer > 0)
        {
            _seconds = ((int)_timer % 60);
            _minutes = ((int)_timer / 60);

            _timer -= Time.deltaTime;
        }

    }

    public void PauseGame()
    {
        Time.timeScale = 0;
    }

    public void ContinueGame()
    {
        Time.timeScale = 1;
    }


    private void IsGamePaused(GameObject pauseScreen)
    {

        if (pauseScreen)
        {
            PauseGame();
        }
        else
        {
            ContinueGame();
        }
    }



}
