using UnityEngine;

public class GameEnder : MonoBehaviour
{
    [SerializeField]
    private GameObject[] _players;

    [SerializeField]
    private float _timer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //pause game when ended
        //redirect to the vizualize winner
    }
}
