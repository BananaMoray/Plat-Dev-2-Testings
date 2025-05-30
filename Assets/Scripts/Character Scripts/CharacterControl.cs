using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;


public class CharacterControl : MonoBehaviour
{
    [Header("Player Data")]
    [SerializeField]
    private Material[] _playerColours;
    private PlayerInput _playerInput;
    private CharacterController _controller;
    private PizzaScoreZone _score;

    [Header("Attack Data")]
    public bool IsHit;
    [SerializeField]
    private float _stunTime = 1f;
    [SerializeField]
    private GameObject _throwCube;
    private float _hitDistance = 1.5f;
    [SerializeField]
    private float _hitForce = 10f;
    [SerializeField]
    private float _attackCooldownTime = 0.8f;
    private float _attackTimer = 0f;
    [SerializeField] 
    private Color _hitColor = Color.red;
    [SerializeField]
    private bool IsBlocking = false;
    [SerializeField]
    private bool _canEarnPointsThroughAttacking;

    [Header("Audio Data")]
    [SerializeField]
    private AudioSource _hitAudio;

    [Header("Input")]
    [SerializeField]
    private float _groundDrag = 1f;
    [SerializeField]
    private float _acceleration = 5f;
    [SerializeField]
    private float _moveSpeedMax = 10f;
    [SerializeField]
    private float _moveSpeedSlowMax = 4f;
    [SerializeField]
    private float _rotationSpeed = 720f;
    private float _minimumInput = 0.1f;
    [SerializeField]
    private GameObject _uiManager;

    //camera stuff
    private Camera _camera;
    private Vector3 _cameraForward;
    private Vector3 _cameraRight;

    [Header("Arm Animator stuff")]
    [SerializeField]
    private Animator _armRightAnimator;
    [SerializeField]
    private Animator _armLeftAnimator;

    //picking up data and variables
    [Header("Pickup and Throw Data")]
    public GameObject HeldTopping;
    private Rigidbody _heldToppingBody;
    private bool _canPickup = true;


    [Header("Trajectory Rendering")]
    private LineRenderer _lineRenderer;
    [SerializeField]
    private int _trajectoryResolution = 30;

    [Header("Topping Data")]
    [SerializeField]
    private float _timeToPickup = 1f;
    private float _pickuptimer;
    [SerializeField]
    private float _pickupDistance = 2f;
    [SerializeField]
    private float _throwForce = 10f;
    [SerializeField]
    private float _timeToFullThrowForce = 3f;
    private float _throwTimer;

    //input system stuff
    private Vector2 _movementInput = Vector2.zero;
    private Vector2 _lookInput = Vector2.zero;
    private bool _interact;
    private bool _fire;
    private bool _pause;

    private void Start()
    {
        //retrieve components
        _lineRenderer = GetComponent<LineRenderer>();
        _controller = GetComponent<CharacterController>();
        _playerInput = GetComponent<PlayerInput>();
        _camera = Camera.main;
        //calculations
        CalculateCameraDirections();
        HandlePlayerColour();
    }

    public void HandlePlayerColour()
    {
        GetComponent<MeshRenderer>().material = _playerColours[_playerInput.playerIndex];
    }

    private void CalculateCameraDirections()
    {
        //grab the camera transforms
        _cameraForward = _camera.transform.forward;
        _cameraRight = _camera.transform.right;

        //just makes sure there is no vertical shenanigans
        _cameraForward.y = 0;
        _cameraRight.y = 0;

        //normalize forward and right direction
        _cameraForward.Normalize();
        _cameraRight.Normalize();
    }
    public void OnMove(InputAction.CallbackContext context)
    {
        _movementInput = context.ReadValue<Vector2>();
    }
    public void OnLook(InputAction.CallbackContext context)
    {
        _lookInput = context.ReadValue<Vector2>();
    }
    public void OnInteract(InputAction.CallbackContext context)
    {
        _interact = context.action.triggered;
    }
    public void OnFire(InputAction.CallbackContext context)
    {
        _fire = context.action.triggered;
    }
    public void OnPause(InputAction.CallbackContext context)
    {
        _pause = context.action.triggered;
    }
    void Update()
    {
        HandleInput();

        _attackTimer += Time.deltaTime;

        if (_controller.enabled)
        {
            HandleGravity();
            ApplyVelocity();
        }

        if (HeldTopping == null)
        {
            _lineRenderer.positionCount = 0;
        }
    }

    private void HandleInput()
    {
        //currentPauseInput = _pause;

        //if (previousPauseInput && !currentPauseInput)
        //{
        //    Debug.Log("pause released");

        //}

        //if (!previousPauseInput && currentPauseInput)
        //{
        //    Debug.Log("pause pressed");
        //    _uiManager.GetComponent<UIManager>().OpenPauseScreen();
        //}

        //previousPauseInput = currentPauseInput;

        if (!IsHit)
        {
            HandleMovement();
            HandleRotation(_lookInput.y, _lookInput.x);
            HandlePickup();
            HandleThrowing();
            HandleAttack();
            HandleBlock();
        }
        else
        {
            ThrowObject();
        }
    }

    private void HandleBlock()
    {

        if (IsBlocking)
        {
            if (HeldTopping != null)
                HeldTopping.transform.localRotation = Quaternion.Euler(90f, 0, 0);
            _throwTimer = 0;
        }
        else
        {
            if (HeldTopping != null)
                HeldTopping.transform.localRotation = new Quaternion(0, 0f, 0f, 0f);
        }
    }

    public void ResetPlayer()
    {
        transform.SetParent(null);
        transform.rotation = Quaternion.identity;
        _controller.enabled = true;
        IsHit = false;
        HandlePlayerColour();
    }

    //we now separate the vertical Y from the horizontal X and Z
    //if we dont do that the gravity would reset every frame
    float verticalVelocity = 0f;
    private Vector3 _velocity = Vector3.zero;

    private void HandleMovement()
    {
        //yes, we recalculate the vector new all the time because otherwise you literally can not stand still
        Vector3 inputDirection = Vector3.zero;

        if (Mathf.Abs(_movementInput.x) >= _minimumInput || Mathf.Abs(_movementInput.y) >= _minimumInput)
        {
            inputDirection = (_cameraForward * _movementInput.y + _cameraRight * _movementInput.x).normalized;
        }

        _velocity += inputDirection * _acceleration;

        ApplyGroundDrag();
        ApplySpeedLimitation();
    }

    private void ApplyGroundDrag()
    {
        if (_controller.isGrounded)
        {
            _velocity *= (1 - Time.deltaTime * _groundDrag);
        }
    }

    private void ApplySpeedLimitation()
    {
        float tempY = _velocity.y;

        _velocity.y = 0;
        if (HeldTopping != null)
        {
            _velocity = Vector3.ClampMagnitude(_velocity, _moveSpeedSlowMax);
        }
        else
        {
            _velocity = Vector3.ClampMagnitude(_velocity, _moveSpeedMax);
        }

        _velocity.y = tempY;
    }

    private void HandleGravity()
    {
        if (_controller.isGrounded)
        {
            //we apply a small downward velocity so the character controller stays grounded
            verticalVelocity = -1f;
        }
        else
        {
            verticalVelocity += gravity.y * Time.deltaTime;
        }
        _velocity.y = verticalVelocity;
    }

    private void ApplyVelocity()
    {
        _controller.Move(_velocity * Time.deltaTime);
    }

    Vector2 moveInput = Vector2.zero;
    Vector2 lookInput = Vector2.zero;

    bool currentPauseInput;
    bool previousPauseInput;

    private void HandleAttack()
    {
        if (IsBlocking && !_fire)
            IsBlocking = false;

        if (_fire && !_interact)
        {
            if (HeldTopping != null)
            {
                IsBlocking = true;
            }
            else if (_attackTimer >= _attackCooldownTime)
            {
                _attackTimer = 0f;
                Debug.Log("Attack!!");
                _armRightAnimator.SetTrigger("Attack");
            }
        }
    }

    public void Attack()
    {
        Vector3 hitOrigin = transform.position + transform.forward * 1.5f;

        Collider[] colliders = Physics.OverlapSphere(hitOrigin, _hitDistance);
        foreach (Collider collider in colliders)
        {
            CharacterControl characterControl = collider.GetComponent<CharacterControl>();
            if (characterControl == null) continue;


            int myIndex = GetComponent<PlayerInput>().playerIndex;
            int otherIndex = characterControl.GetComponent<PlayerInput>().playerIndex;

            if (myIndex == otherIndex) continue;

            if (characterControl.IsHit) continue;

            if (characterControl.IsBlocking)
            {
                StunSelf();
                continue;
            }

            _hitAudio.Play();
            if (_canEarnPointsThroughAttacking)
            {
                PizzaScoreZone.PlayerScores[_playerInput.playerIndex]++;
            }

            LaunchPlayer(characterControl.gameObject, characterControl);
            break;
        }
    }

    private void StunSelf()
    {
        IsHit = true;
        _velocity = Vector3.zero;
        _hitAudio.Play();
        GetComponent<MeshRenderer>().material = _playerColours[GetComponent<PlayerInput>().playerIndex + 4];
        StartCoroutine(StunPlayer(_stunTime));
    }

    IEnumerator StunPlayer(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        ResetPlayer();
    }

    private void LaunchPlayer(GameObject hitPlayer, CharacterControl characterControl)
    {
        GameObject cube = Instantiate(_throwCube, hitPlayer.transform.position, Quaternion.identity);

        characterControl.IsHit = true;
        hitPlayer.transform.SetParent(cube.transform);

        objectVelocity = new Vector3(transform.forward.x, 0.5f, transform.forward.z).normalized * _hitForce;

        cube.GetComponent<Rigidbody>().AddForce(objectVelocity, ForceMode.Impulse);

        hitPlayer.GetComponent<CharacterController>().enabled = false;

        hitPlayer.GetComponent<MeshRenderer>().material = _playerColours[hitPlayer.GetComponent<PlayerInput>().playerIndex + 4];

    }


    private void HandlePickup()
    {
        if (_canPickup)
        {
            TryPickupObject();
        }

        if (!_canPickup && HeldTopping == null)
        {
            _pickuptimer += Time.deltaTime;
            if (_pickuptimer >= _timeToPickup)
            {
                _pickuptimer = 0f;
                //Debug.Log("You can pick things up again");
                _canPickup = true;
            }
        }
    }

    private void HandleThrowing()
    {
        if (!IsBlocking && HeldTopping != null)
        {
            if (_interact)
            {
                _throwTimer += Time.deltaTime;
                if (_throwTimer >= _timeToFullThrowForce)
                {
                    //Debug.Log("Fully charged");
                    //_throwTimer = _timeToFullThrowForce;
                    ThrowObject();
                    _throwTimer = 0f;
                }
                DrawThrowTrajectoryInGameView();
            }
            if (!_interact && _throwTimer > _minimumInput)
            {
                ThrowObject();
                _throwTimer = 0f;
            }
        }
    }

    Vector3 lookDirection = Vector3.zero;
    private void HandleRotation(float vertical, float horizontal)
    {
        //if no input, turn into the walk direction
        if (Mathf.Abs(horizontal) < _minimumInput && Mathf.Abs(vertical) < _minimumInput)
        {
            lookDirection = _cameraForward * _movementInput.y + _cameraRight * _movementInput.x;
        }
        else
        {
            lookDirection = _cameraForward * vertical + _cameraRight * horizontal;
        }

        //shamelessly stolen from a tutorial
        if (lookDirection.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, _rotationSpeed * Time.deltaTime);
        }
    }

    private void TryPickupObject()
    {
        //Debug.Log("triggered");
        Collider[] colliders = Physics.OverlapSphere(transform.position, _pickupDistance);
        foreach (Collider collider in colliders)
        {
            ToppingHandler topping = collider.GetComponent<ToppingHandler>();

            if (topping == null) continue;

            if (!topping.CanBePickedUp)
            {
                continue;
            }

            if (topping.PlayerIndex != _playerInput.playerIndex || !topping.CanBePickedUp) continue;

            AssignHeldObject(collider.gameObject);
            HoldObject(true);
            _canPickup = false;
            break;
        }
    }

    private void AssignHeldObject(GameObject obj)
    {
        HeldTopping = obj;
        HeldTopping.transform.SetParent(transform);
        _heldToppingBody = obj.GetComponent<Rigidbody>();
        _heldToppingBody.transform.localPosition = new Vector3(0, 1, 1);
    }

    private void ThrowObject()
    {
        if (HeldTopping == null) return;

        HoldObject(false);

        
        HeldTopping.transform.SetParent(null);

        objectVelocity = new Vector3(transform.forward.x, 0.5f, transform.forward.z).normalized * CalculateThrowForce();

        _heldToppingBody.AddForce(objectVelocity, ForceMode.Impulse);

        HeldTopping = null;
        _canPickup = false;
    }
    private void HoldObject(bool v)
    {
        if (_heldToppingBody != null)
        {
            _heldToppingBody.useGravity = !v;
            _heldToppingBody.isKinematic = v;
        }

    }

    private float CalculateThrowForce()
    {
        return _throwForce * (_throwTimer / _timeToFullThrowForce);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position + transform.forward * 1.5f, _hitDistance);


        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + lookDirection * 4);

        if (HeldTopping == null) return;

        DrawThrowTrajectoryInSceneView();
    }

    //welcome new gizmo tools
    Vector3 startPosition = Vector3.zero;
    Vector3 objectVelocity = Vector3.zero;
    float gizmoTimeStep = 0.1f;
    Vector3 gravity = new Vector3(0, -9.81f, 0);

    private void DrawThrowTrajectoryInGameView()
    {
        if (_lineRenderer == null)
            return; //use line renderer pls

        //there's an akward nullException when the object is being thrown automatically
        //now we simply tell this function not to do anything if there is no heldobject
        if (_heldToppingBody == null)
            return;

        Vector3[] points = new Vector3[_trajectoryResolution];

        startPosition = HeldTopping.transform.position;
        objectVelocity = new Vector3(transform.forward.x, 0.5f, transform.forward.z).normalized * CalculateThrowForce();

        for (int i = 0; i < _trajectoryResolution; i++)
        {
            float time = i * gizmoTimeStep;
            Vector3 position = startPosition + objectVelocity * time + 0.5f * gravity * (time * time);
            if (position.y < -2) position.y = -2;
            points[i] = position;
        }

        _lineRenderer.positionCount = _trajectoryResolution;
        _lineRenderer.SetPositions(points);
    }


    private void DrawThrowTrajectoryInSceneView()
    {

        //Vector3 throwGizmo = new Vector3(transform.forward.x, 0.5f, transform.forward.z) * 10;
        //Gizmos.DrawLine(_heldObject.position, _heldObject.position + throwGizmo * _throwTimer / _timeToFullThrowForce);

        startPosition = HeldTopping.transform.position;

        objectVelocity = new Vector3(transform.forward.x, 0.5f, transform.forward.z).normalized * CalculateThrowForce();

        Gizmos.color = Color.green;

        for (float time = 0; time < _timeToFullThrowForce; time += gizmoTimeStep)
        {
            //welcome back projectile motion you awful thing
            Vector3 position = startPosition + objectVelocity * time + 0.5f * gravity * (time * time);
            //let's draw small spheres yay
            Gizmos.DrawSphere(position, 0.1f);
        }
    }
}

