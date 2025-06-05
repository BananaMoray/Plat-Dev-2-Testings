using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
    [SerializeField]
    private Color _IconColorWhenActive;
    [SerializeField]
    private Color _IconColorWhenNotActive;

    [SerializeField]
    private float _punchingTimer = 0;
    [SerializeField]
    private float _punchingDuration = 0.5f;


    void Start()
    {
        for (int i = 0; i < _countOfTokens; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {


        ShowingPunchAndShield();

        // if is Blocking
        if (_playerIndex > _queueDelayManager.Players.Length - 1)
        {
            { }// empty block to avoid index out of range
        }
        else if (_queueDelayManager.Players[_playerIndex].GetComponent<CombatHandler>().IsBlocking)
        {
            transform.GetChild(1).gameObject.GetComponent<RawImage>().color = _IconColorWhenActive;
            transform.GetChild(3).gameObject.GetComponent<RawImage>().color = _IconColorWhenActive;
        }
        else
        {
            transform.GetChild(1).gameObject.GetComponent<RawImage>().color = _IconColorWhenNotActive;
            transform.GetChild(3).gameObject.GetComponent<RawImage>().color = _IconColorWhenNotActive;
        }


        // if is Punching
        if (_playerIndex > _queueDelayManager.Players.Length - 1)
        {
            { }//empty block to avoid index out of range
        }
        else if (_queueDelayManager.Players[_playerIndex].GetComponent<CombatHandler>().IsPunching)
        {
            _punchingTimer += Time.deltaTime;
            transform.GetChild(0).gameObject.GetComponent<RawImage>().color = _IconColorWhenActive;
            transform.GetChild(3).gameObject.GetComponent<RawImage>().color = _IconColorWhenActive;
            if (_punchingTimer >= _punchingDuration)
            {
                _punchingTimer = 0;
                _queueDelayManager.Players[_playerIndex].GetComponent<CombatHandler>().IsPunching = false;
            }
        }
        else
        {
            transform.GetChild(0).gameObject.GetComponent<RawImage>().color = _IconColorWhenNotActive;
            if (!_queueDelayManager.Players[_playerIndex].GetComponent<CombatHandler>().IsBlocking)
                transform.GetChild(3).gameObject.GetComponent<RawImage>().color = _IconColorWhenNotActive;
        }


        // if is Throwing
        if (_playerIndex > _queueDelayManager.Players.Length - 1)
        {
            { }//empty block to avoid index out of range
        }
        else if (_queueDelayManager.Players[_playerIndex].GetComponent<PickupHandler>().IsThrowing)
        {
            transform.GetChild(2).gameObject.GetComponent<RawImage>().color = _IconColorWhenActive;
        }
        else
        {
            transform.GetChild(2).gameObject.GetComponent<RawImage>().color = _IconColorWhenNotActive;
        }
    }

    private void ShowingPunchAndShield()
    {
        if (_playerIndex > _queueDelayManager.Players.Length - 1)
        {
            transform.GetChild(1).gameObject.SetActive(false);
            transform.GetChild(0).gameObject.SetActive(false);
            transform.GetChild(3).gameObject.SetActive(false);
            transform.GetChild(2).gameObject.SetActive(false);
        }
        else if (_queueDelayManager.Players[_playerIndex].GetComponent<PickupHandler>().IsHolding)
        {
            transform.GetChild(1).gameObject.SetActive(true);
            transform.GetChild(0).gameObject.SetActive(false);
            transform.GetChild(3).gameObject.SetActive(true);
            transform.GetChild(2).gameObject.SetActive(true);
        }
        else
        {
            transform.GetChild(1).gameObject.SetActive(false);
            transform.GetChild(0).gameObject.SetActive(true);
            transform.GetChild(3).gameObject.SetActive(true);
            transform.GetChild(2).gameObject.SetActive(false);
        };
    }
}
