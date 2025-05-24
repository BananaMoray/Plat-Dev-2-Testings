
using System.Runtime.CompilerServices;
using UnityEngine;

public class PunchShieldVisualizationManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField]
    private GameObject[] _playerShields;


    [SerializeField]
    private QueueDelay _queueDelayManager;


    [SerializeField]
    private float _shieldHeightOffset;

    void Start()
    {
        foreach(GameObject shield in _playerShields)
        {
            shield.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        int countOfIndex = 0;
        foreach (GameObject player in _queueDelayManager.Players)
        {
            if (_queueDelayManager.Players[countOfIndex].GetComponent<CombatHandler>().IsBlocking)
            {
                _playerShields[countOfIndex].SetActive(true);
                _playerShields[countOfIndex].transform.position = player.transform.position + new Vector3(0, _shieldHeightOffset, -0.01f);
            }
            else
            {
                _playerShields[countOfIndex].SetActive(false);
            }
            countOfIndex++;
        }
    }
}
