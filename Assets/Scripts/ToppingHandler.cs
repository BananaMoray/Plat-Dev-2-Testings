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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _rigidBody = GetComponent<Rigidbody>();
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
        if (IsScored)
        {
            CanBePickedUp = false;
        }
    }
}
