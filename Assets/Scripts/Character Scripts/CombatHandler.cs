using UnityEngine;
using UnityEngine.InputSystem;

public class CombatHandler : MonoBehaviour
{
    [Header("Attack Data")]
    [SerializeField]
    private float _hitDistance = 1.5f;
    [SerializeField]
    private float _hitForce = 10f;
    [SerializeField]
    private GameObject _throwCube;
    [Header("Audio Data")]
    [SerializeField]
    private AudioSource _hitAudio;

    private PlayerInput _playerInput;
    private CharacterController _characterController;
    public bool IsHit;

    private Vector3 _objectVelocity = Vector3.zero;

    private float _attackCooldownTime = 1f;
    private float _attackTimer = 0f;

    private void Awake()
    {
        _playerInput = GetComponent<PlayerInput>();
        _characterController = GetComponent<CharacterController>();
    }

    public void Update()
    {
        _attackTimer += Time.deltaTime;
    }

    public void HandleAttack(bool fireInput, bool isHolding)
    {
        if (isHolding || IsHit) return;

        if (fireInput)
        {
            if (_attackTimer >= _attackCooldownTime)
            {

                _attackTimer = 0;
                Vector3 hitOrigin = transform.position + transform.forward * 1.5f;
                Collider[] colliders = Physics.OverlapSphere(hitOrigin, _hitDistance);

                foreach (Collider collider in colliders)
                {
                    CombatHandler otherCollision = collider.GetComponent<CombatHandler>();
                    if (otherCollision == null || otherCollision.IsHit) continue;

                    int myIndex = _playerInput.playerIndex;
                    int otherIndex = collider.GetComponent<PlayerInput>().playerIndex;

                    if (myIndex == otherIndex) continue;

                    LaunchPlayer(collider.gameObject, otherCollision);
                    _hitAudio.Play();
                    break;
                }
            }
        }
    }

    private void LaunchPlayer(GameObject hitPlayer, CombatHandler targetHandler)
    {
        GameObject throwCube = Instantiate(_throwCube, hitPlayer.transform.position, Quaternion.identity);
        targetHandler.IsHit = true;
        hitPlayer.transform.SetParent(throwCube.transform);

        _objectVelocity = (transform.forward + Vector3.up * 0.5f).normalized * _hitForce;
        throwCube.GetComponent<Rigidbody>().AddForce(_objectVelocity, ForceMode.Impulse);

        hitPlayer.GetComponent<CharacterController>().enabled = false;
    }

    public void ResetPlayer()
    {
        transform.SetParent(null);
        transform.rotation = Quaternion.identity;
        _characterController.enabled = true;
        IsHit = false;
    }
}
