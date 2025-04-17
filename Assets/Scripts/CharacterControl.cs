using System;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEditor.Searcher.SearcherWindow.Alignment;
using UnityEngine.InputSystem.XR;
using UnityEditor;
using Unity.VisualScripting;

public class CharacterControl : MonoBehaviour
{
    [Header("Input")]
    [SerializeField]
    private float _moveSpeed = 10f;
    [SerializeField]
    private float _rotationSpeed = 720f;
    private float _minimumInput = 0.1f;


    [Header("Player Data")]
    [SerializeField]
    private Material[] _playerColours;
    private PlayerInput _playerInput;
    private CharacterController _controller;
    public bool IsHit;

    //camera stuff
    private Camera _camera;
    private Vector3 _cameraForward;
    private Vector3 _cameraRight;


    //picking up data and variables
    [Header("Pickup and Throw Data")]
    public GameObject HeldTopping;
    private Rigidbody _heldToppingBody;
    private bool _canPickup = true;


    [Header("Trajectory Rendering")]
    private LineRenderer _lineRenderer;
    [SerializeField]
    private int _trajectoryResolution = 30;


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

    private void Start()
    {
        //retrieve components
        _lineRenderer = GetComponent<LineRenderer>();
        _controller = GetComponent<CharacterController>();
        _playerInput = GetComponent<PlayerInput>();
        _camera = Camera.main;
        //calculations
        CalculateCameraDirections();
        HandlePlayerColour(_playerInput.playerIndex);
    }

    private void HandlePlayerColour(int playerIndex)
    {
        GetComponent<MeshRenderer>().material = _playerColours[playerIndex];
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
    void Update()
    {
        if (!IsHit)
        {
            HandleInput();
        }

        HandleGravity();
        ApplyMovement();

        if (HeldTopping == null)
        {
            _lineRenderer.positionCount = 0;
        }
    }

    //we now separate the vertical Y from the horizontal X and Z
    //if we dont do that the gravity would reset every frame
    float verticalVelocity = 0f;
    Vector3 moveDirection = Vector3.zero;

    private void HandleMovement(float vertical, float horizontal)
    {
        //yes, we recalculate the vector new all the time because otherwise you literally can not stand still
        Vector3 inputDirection = Vector3.zero;

        if (Mathf.Abs(horizontal) >= _minimumInput || Mathf.Abs(vertical) >= _minimumInput)
        {
            inputDirection = (_cameraForward * vertical + _cameraRight * horizontal).normalized;
        }
        moveDirection = inputDirection * _moveSpeed;
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
        moveDirection.y = verticalVelocity;
    }

    private void ApplyMovement()
    {
        _controller.Move(moveDirection * Time.deltaTime);
    }

    Vector2 moveInput = Vector2.zero;
    Vector2 lookInput = Vector2.zero;
    private void HandleInput()
    {
        HandleMovement(_movementInput.y, _movementInput.x);
        HandleRotation(_lookInput.y, _lookInput.x);
        HandlePickup();
        HandleThrowing();
        HandleAttack();
    }


    private void HandleAttack()
    {
        if (HeldTopping == null)
        {
            if (_fire)
            {
                Collider[] colliders = Physics.OverlapSphere(transform.position + transform.forward, _pickupDistance);
                foreach (Collider collider in colliders)
                {
                    CharacterControl characterControl = collider.GetComponent<CharacterControl>();
                    GameObject hitPlayer = characterControl.gameObject;

                    if (characterControl == null) continue;

                    if (GetComponent<PlayerInput>().playerIndex == _playerInput.playerIndex || characterControl.IsHit) continue;

                    LaunchPlayer(hitPlayer, characterControl);

                    break;
                }

            }
        }
    }

    private void LaunchPlayer(GameObject hitPlayer, CharacterControl characterControl)
    {
        return;
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
        if (HeldTopping != null)
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
        HoldObject(false);

        HeldTopping.transform.SetParent(null);

        velocity = new Vector3(transform.forward.x, 0.5f, transform.forward.z).normalized * CalculateThrowForce();

        _heldToppingBody.AddForce(velocity, ForceMode.Impulse);

        HeldTopping = null;
        _canPickup = false;
    }
    private void HoldObject(bool v)
    {
        _heldToppingBody.useGravity = !v;
        _heldToppingBody.isKinematic = v;
    }

    private float CalculateThrowForce()
    {
        return _throwForce * (_throwTimer / _timeToFullThrowForce);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + lookDirection * 4);

        if (_heldToppingBody != null)
        {
            DrawThrowTrajectoryInSceneView();
        }
        //Gizmos.DrawSphere(transform.position, _pickupDistance);
    }

    //welcome new gizmo tools
    Vector3 startPosition = Vector3.zero;
    Vector3 velocity = Vector3.zero;
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

        startPosition = _heldToppingBody.position;
        velocity = new Vector3(transform.forward.x, 0.5f, transform.forward.z).normalized * CalculateThrowForce();

        for (int i = 0; i < _trajectoryResolution; i++)
        {
            float time = i * gizmoTimeStep;
            Vector3 position = startPosition + velocity * time + 0.5f * gravity * (time * time);
            if (position.y < -2) position.y = -2;
            points[i] = position;
        }

        _lineRenderer.positionCount = _trajectoryResolution;
        _lineRenderer.SetPositions(points);
    }


    private void DrawThrowTrajectoryInSceneView()
    {
        //the old trajectory code

        //Gizmos.color = Color.red;
        //Vector3 throwGizmo = new Vector3(transform.forward.x, 0.5f, transform.forward.z) * 10;
        //Gizmos.DrawLine(_heldObject.position, _heldObject.position + throwGizmo * _throwTimer / _timeToFullThrowForce);

        startPosition = _heldToppingBody.position;

        velocity = new Vector3(transform.forward.x, 0.5f, transform.forward.z).normalized * CalculateThrowForce();

        Gizmos.color = Color.green;

        for (float time = 0; time < _timeToFullThrowForce; time += gizmoTimeStep)
        {
            //welcome back projectile motion you awful thing
            Vector3 position = startPosition + velocity * time + 0.5f * gravity * (time * time);
            //let's draw small spheres yay
            Gizmos.DrawSphere(position, 0.1f);
        }
    }
}

