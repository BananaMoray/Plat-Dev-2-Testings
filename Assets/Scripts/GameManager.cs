using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{

    [SerializeField]
    TMP_Text _timerVisuals;

    [SerializeField] //in seconds
    float _timer;

    [SerializeField]
    float _seconds, _minutes;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {



    }

    // Update is called once per frame
    void Update()
    {
        Timer();
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



}
