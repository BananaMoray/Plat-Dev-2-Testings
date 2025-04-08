using System.Linq;
using UnityEngine;

public class ToppingHandler : MonoBehaviour
{

    [SerializeField]
    private Material[] _toppingColours;

    public int PlayerIndex;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        PlayerIndex = Random.Range(0, _toppingColours.Count());
        HandleColour(PlayerIndex);
    }

    private void HandleColour(int colour)
    {
        GetComponent<MeshRenderer>().material = _toppingColours[colour];
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
