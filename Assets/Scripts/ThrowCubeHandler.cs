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

    // Update is called once per frame
    void Update()
    {

        if (_rigidbody.linearVelocity.magnitude <= 0.01f)
        {
            _player.transform.SetParent(null);
            _player.transform.rotation = Quaternion.identity;
            _player.GetComponent<CharacterController>().enabled = true;
            _player.GetComponent<CharacterControl>().IsHit = false;
            Destroy(this.gameObject);
        }
    }
}
