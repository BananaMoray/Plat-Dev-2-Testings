using UnityEngine;

public class ThrowCubeHandler : MonoBehaviour
{

    private Rigidbody _rigidbody;
    private GameObject _player;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _player = transform.GetChild(0).gameObject;
    }

    Vector3 currentPos = Vector3.zero;
    Vector3 previousPos = Vector3.zero;


    // Update is called once per frame
    void Update()
    {
        currentPos = transform.position;

        if (_rigidbody.linearVelocity.magnitude <= 0.01f)
        {
            _player.GetComponent<CharacterControl>().ResetPlayer();
            Destroy(this.gameObject);
        }

        previousPos = currentPos;
    }
}
