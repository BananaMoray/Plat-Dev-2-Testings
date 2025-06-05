using System.Linq;
using UnityEngine;

public class AdjustScoreToPlayerNumber : MonoBehaviour
{
    [SerializeField] private QueueDelay _quedelayManager;

    [SerializeField] private GameObject _thirdScore;
    [SerializeField] private GameObject _thirdScoreBox;
    [SerializeField] private GameObject _thirdScoreFinal;



    [SerializeField] private GameObject _fourthScore;
    [SerializeField] private GameObject _fourthScoreBox;
    [SerializeField] private GameObject _fourthScoreFinal;
    // Start is called once before the first execution of Update after the MonoBehaviour is created


    // Update is called once per frame
    void Update()
    {
        Debug.Log(_quedelayManager?.Players.Count());
        AdjustScore();
    }
    private void AdjustScore()
    {
        if (_quedelayManager.Players == null)
            return;

        if (_quedelayManager.Players.Count() < 4)
        {
            _fourthScoreBox.SetActive(false);
            _fourthScore.SetActive(false);
            _fourthScoreFinal.SetActive(false);
        }
        if (_quedelayManager.Players.Count() < 3)
        {
            _fourthScore.SetActive(false);
            _fourthScoreBox.SetActive(false);
            _fourthScoreFinal.SetActive(false);

            _thirdScore.SetActive(false);
            _thirdScoreBox.SetActive(false);
            _thirdScoreFinal.SetActive(false);
        }


    }
}
