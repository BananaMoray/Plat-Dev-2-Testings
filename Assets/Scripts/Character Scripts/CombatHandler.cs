using System;
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
    [SerializeField, Range(0, 10)]
    private float _torqueMultiplier = 1f;
    [SerializeField]
    private int _hitValue = 1;
    [SerializeField]
    private GameObject _throwCube;
    [SerializeField]
    private bool _canEarnPointsThroughAttacking = true;
    [SerializeField]
    private float _hitStunTime = 2f;
    [SerializeField]
    private float _blockStunTime = 2f;

    [Header("Component Data")]
    [SerializeField]
    private AudioSource _hitAudio;
    private Animator _anim;

    [SerializeField]
    private GameObject _textObject;

    private PlayerInput _playerInput;
    private CharacterController _characterController;
    private CharacterManager _characterManager;

    public bool IsHit;
    public bool IsBlocking;

    private Vector3 _objectVelocity = Vector3.zero;

    private float _attackCooldownTime = 1f;
    private float _attackTimer = 0f;

    [Header("Shield Data")]

    [SerializeField]
    private GameObject _shieldObject;
    [SerializeField]
    private GameObject _shieldUI;
    [SerializeField]
    private GameObject _shieldSlider;
    [SerializeField]
    private Vector3 _shieldOffset = new Vector3(0, 0.5f, -1f);

    [SerializeField, Range(0, 10)]
    private float _shieldDuration;
    private float _shieldTimer = 0f;

    [SerializeField, Range(0, 10)]
    private float _shieldCooldownDuration;
    private float _shieldCooldownTime = 1000f;
    //private bool _shieldTimerActive = false;

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

        HandleShield(IsBlocking);

        if (_shieldCooldownTime >= _shieldCooldownDuration)
        {
            if (isHolding && fireInput && _shieldTimer < _shieldDuration)
            {
                IsBlocking = true;
                _shieldTimer += Time.deltaTime;
                //reset shield timer if it is not active yet

                if (_shieldTimer > _shieldDuration)
                {
                    _shieldCooldownTime = 0f;
                }
            }
            else
            {
                IsBlocking = false;

                if (_shieldTimer > 0.01f) _shieldTimer -= Time.deltaTime;
            }
        }
        else
        {
            _shieldCooldownTime += Time.deltaTime;
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

    private void HandleShield(bool isTrue)
    {
        _shieldObject.GetComponent<MeshRenderer>().enabled = isTrue;
        //_shieldUI.SetActive(isTrue);
        _shieldSlider.transform.localScale = new Vector3(CalculateCurrentShieldDuration(), 1, 1);
        _shieldUI.transform.rotation = Quaternion.identity;

        if (isTrue)
        {
            _shieldObject.transform.position = gameObject.transform.position + _shieldOffset;
            _shieldObject.transform.rotation = Quaternion.Euler(90f, 0, 0);
            //_shieldUI.transform.LookAt(Camera.main.transform.position);
            
        }

    }

    private float CalculateCurrentShieldDuration()
    {
        return 1 - _shieldTimer / _shieldDuration;
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
            if (_canEarnPointsThroughAttacking && QueueDelay.IsStarted)
            {
                GameObject text = Instantiate(_textObject, gameObject.transform.position + Vector3.up * 2, Quaternion.identity);
                FloatingTextItem textItem = text.GetComponent<FloatingTextItem>();
                textItem.TMP.SetText("+" + _hitValue);
                textItem.HandleTextColour(_characterManager.PlayerIndex);

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
        throwCube.GetComponent<ThrowCubeHandler>()._timeToReset = _hitStunTime;
        throwCube.GetComponent<Rigidbody>().AddForce(_objectVelocity, ForceMode.Impulse);
        throwCube.GetComponent<Rigidbody>().AddTorque(RandomizeTorque(_torqueMultiplier), ForceMode.Impulse);

        hitPlayer.GetComponent<CharacterController>().enabled = false;
        hitPlayer.GetComponent<CharacterMovement>().enabled = false;
    }

    private Vector3 RandomizeTorque(float multiplier)
    {
        float x = UnityEngine.Random.Range(-1, 1);
        float y = UnityEngine.Random.Range(-1, 1);
        float z = UnityEngine.Random.Range(-1, 1);

        return new Vector3(x, y, z) * multiplier;
    }

    private void StunSelf()
    {
        IsHit = true;
        GetComponent<CharacterMovement>().enabled = false;
        GetComponent<CharacterManager>().HandlePlayerColour(_playerInput.playerIndex + 4);
        _hitAudio.Play();

        StartCoroutine(StunPlayer(_blockStunTime));
    }

    IEnumerator StunPlayer(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        _characterManager.ResetPlayer();
    }


}
