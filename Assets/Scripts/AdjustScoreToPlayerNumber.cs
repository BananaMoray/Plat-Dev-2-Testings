using System.Linq;
using UnityEngine;

public class AdjustScoreToPlayerNumber : MonoBehaviour
{
    [SerializeField] QueueDelay _quedelayManager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        Debug.Log(_quedelayManager?.Players.Count());
    }
}
