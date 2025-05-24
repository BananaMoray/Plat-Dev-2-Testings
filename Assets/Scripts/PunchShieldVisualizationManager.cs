
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class PunchShieldVisualizationManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [Header("Punch Shield Visualization Manager")]


    private GameObject[] _playerShields = new GameObject[4];

    [SerializeField]
    private QueueDelay _queueDelayManager;


    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        int countOfIndex = 0;
        foreach (GameObject playerpunchAndShield in _playerShields)
        {
            if (_queueDelayManager.Players[countOfIndex].GetComponent<CombatHandler>().IsBlocking)
            {
                _playerShields[countOfIndex].SetActive(true);
            }

            countOfIndex++;

        }
    }
}
