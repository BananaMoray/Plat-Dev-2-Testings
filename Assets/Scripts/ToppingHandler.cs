using System.Linq;
using UnityEngine;

public class ToppingHandler : MonoBehaviour
{

    [SerializeField]
    private Material[] _toppingColours;

    private Rigidbody _rigidBody;

    public int PlayerIndex;
    public bool IsScored = false;
    public bool CanBePickedUp = true;
    public bool IsPickedUp = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _rigidBody = GetComponent<Rigidbody>();
        HandleColour(PlayerIndex);
    }

    private int CheckPlayercount()
    {
        int activePlayerCount = GameObject.FindGameObjectsWithTag("Player").Length;
        return Mathf.Max(1, activePlayerCount);
         
    }

    private void HandleColour(int colour)
    {

        GetComponent<MeshRenderer>().material = _toppingColours[colour];
        
    }

    // Update is called once per frame
    void Update()
    {


        if (IsScored)
        {
            CanBePickedUp = false;
            GetComponent<MeshRenderer>().material = _toppingColours[PlayerIndex+4];
        }
        else
        {
            CanBePickedUp = true;
            GetComponent<MeshRenderer>().material = _toppingColours[PlayerIndex];
        }


    }

}
