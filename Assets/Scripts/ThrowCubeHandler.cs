using UnityEngine;

public class ThrowCubeHandler : MonoBehaviour
{

    private Rigidbody _rigidbody;
    private GameObject _player;

    [SerializeField]
    public float _timeToReset  =3f;
    private float _resetTimer;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _player = transform.GetChild(0).gameObject;
        
    }

    // Update is called once per frame
    void Update()
    {
        _resetTimer += Time.deltaTime;

        if (_resetTimer >= _timeToReset)
        {
            _resetTimer = 0;
            if (_player.GetComponent<CharacterManager>() != null)
            _player.GetComponent<CharacterManager>().ResetPlayer();
            if (_player.GetComponent<CharacterControl>() != null)
            {
                _player.GetComponent<CharacterControl>().ResetPlayer();
                _player.GetComponent<CharacterControl>().HandlePlayerColour();
            }
            Destroy(gameObject);
        }

    }
}
