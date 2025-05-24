using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class FloatingTextItem : MonoBehaviour
{
    public TMP_Text TMP;
    [SerializeField]
    private float _timeUntilDisappear = 1.5f; 
    [SerializeField]
    private float _riseSpeed = 0.2f;

    private void Awake()
    {
        if (TMP == null) 
            TMP = GetComponentInChildren<TMP_Text>();

        //transform.LookAt(Camera.main.transform.position);

        StartCoroutine(RemoveAfterSetTime());
        
    }

    private void Update()
    {
        transform.position += Vector3.up * _riseSpeed * Time.deltaTime;
    }

    public void HandleTextColour(int i)
    {
        switch (i)
        {
            case 0:
                TMP.color = Color.green;
                break;
            case 1:
                TMP.color = Color.white;
                break;
            case 2:
                TMP.color = Color.red;
                break;
            case 3:
                TMP.color = Color.yellow;
                break;
        }
    }

    IEnumerator RemoveAfterSetTime()
    {
        yield return new WaitForSeconds(_timeUntilDisappear);
        Debug.Log("Destroy");
        Destroy(gameObject);
    }
}
