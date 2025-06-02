using UnityEngine;

public class punchShieldPopUpVisualizationHandler : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [Range(0, 4)]
    [SerializeField]
    private int _playerIndex;

    [SerializeField]
    private int _countOfTokens = 2;
    [SerializeField]
    private QueueDelay _queueDelayManager;

    void Start()
    {
        for (int i =0; i<_countOfTokens; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (_playerIndex > _queueDelayManager.Players.Length -1)
        {
            transform.GetChild(1).gameObject.SetActive(false);
            transform.GetChild(0).gameObject.SetActive(false);
        }
        else if (_queueDelayManager.Players[_playerIndex].GetComponent<PickupHandler>().IsHolding)
        {
            transform.GetChild(1).gameObject.SetActive(true);
            transform.GetChild(0).gameObject.SetActive(false);
        }
        else
        {
            transform.GetChild(1).gameObject.SetActive(false);
            transform.GetChild(0).gameObject.SetActive(true);
        };
        
    }
}
