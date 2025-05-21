using System.Collections;
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
    [SerializeField]
    private bool _canEarnPointsThroughAttacking = true;
    [SerializeField]
    private float _stunTime = 1f;

    [Header("Component Data")]
    [SerializeField]
    private AudioSource _hitAudio;
    private Animator _anim;

    private PlayerInput _playerInput;
    private CharacterController _characterController;
    private CharacterManager _characterManager;

    public bool IsHit;
    public bool IsBlocking;

    private Vector3 _objectVelocity = Vector3.zero;

    private float _attackCooldownTime = 1f;
    private float _attackTimer = 0f;

    private void Awake()
    {
        _playerInput = GetComponent<PlayerInput>();
        _characterController = GetComponent<CharacterController>();
        _anim = GetComponentInChildren<Animator>();
        _characterManager = GetComponent<CharacterManager>();
    }

    public void Update()
    {
        _attackTimer += Time.deltaTime;
    }

    public void HandleAttack(bool fireInput, bool isHolding)
    {
        //if (IsBlocking && !fireInput)
        //    IsBlocking = false;

        if (isHolding && fireInput)
        {
            IsBlocking = true;
        }
        else
        {
            IsBlocking = false;
        }

        if (IsHit) return;

        if (fireInput && !isHolding)
        {
            if (_attackTimer >= _attackCooldownTime)
            {
                _anim.SetTrigger("Attack");
                
                _attackTimer = 0;
            }
        }
    }

    public void Attack()
    {
        Vector3 hitOrigin = transform.position + transform.forward * 1.5f;

        Collider[] colliders = Physics.OverlapSphere(hitOrigin, _hitDistance);
        foreach (Collider collider in colliders)
        {
            CombatHandler otherCombat = collider.GetComponent<CombatHandler>();
            if (otherCombat == null) continue;


            int myIndex = GetComponent<PlayerInput>().playerIndex;
            int otherIndex = otherCombat.GetComponent<PlayerInput>().playerIndex;

            if (myIndex == otherIndex) continue;

            if (otherCombat.IsHit) continue;

            if (otherCombat.IsBlocking)
            {
                StunSelf();
                continue;
            }

            _hitAudio.Play();
            if (_canEarnPointsThroughAttacking)
            {
                PizzaScoreZone.PlayerScores[_playerInput.playerIndex]++;
            }

            otherCombat.gameObject.GetComponent<CharacterManager>().HandlePlayerColour(otherIndex + 4);
            LaunchPlayer(otherCombat.gameObject, otherCombat);
            break;
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
        hitPlayer.GetComponent<CharacterMovement>().enabled = false;
    }

    private void StunSelf()
    {
        IsHit = true;
        GetComponent<CharacterMovement>().enabled = false;
        GetComponent<CharacterManager>().HandlePlayerColour(_playerInput.playerIndex + 4);
        _hitAudio.Play();
        
        StartCoroutine(StunPlayer(_stunTime));
    }

    IEnumerator StunPlayer(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        _characterManager.ResetPlayer();
    }


}
