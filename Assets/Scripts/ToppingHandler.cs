using System.Linq;
using UnityEngine;

public class ToppingHandler : MonoBehaviour
{

    [SerializeField]
    private Material[] _toppingColours;
    [SerializeField]
    private GameObject[] _toppingModels;

    private Rigidbody _rigidBody;

    public int PlayerIndex;
    public bool IsScored = false;
    public bool CanBePickedUp = true;
    public bool IsPickedUp = false;
    public int Value = 3;

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

    public void ChangeIndex(int index)
    {
        PlayerIndex = index;
        HandleColour(PlayerIndex);
    }
    private void HandleColour(int colour)
    {
        //GetComponent<MeshRenderer>().material = _toppingColours[colour];
        foreach (var model in _toppingModels)
        {
            model.SetActive(false);
        }
        _toppingModels[colour].SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        
        if (IsScored)
        {
            CanBePickedUp = false;
            _toppingModels[PlayerIndex].GetComponent<MeshRenderer>().material = _toppingColours[1];

        }
        else
        {
            CanBePickedUp = true;
            _toppingModels[PlayerIndex].GetComponent<MeshRenderer>().material = _toppingColours[0];
        }


    }

}
