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


    //camera stuff
    private Camera _camera;
    private Vector3 _cameraForward;
    private Vector3 _cameraRight;


    //picking up data and variables
    [Header("Pickup and Throw Data")]
    private Rigidbody _heldObject;
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
        HandleInput();
        HandleMovement(_movementInput.y, _movementInput.x);
        HandleRotation(_lookInput.y, _lookInput.x);
        HandleGravity();
        ApplyMovement(moveDirection);


        if (_heldObject == null)
        {
            _lineRenderer.positionCount = 0;
        }
    }

    private void HandleGravity()
    {
        if (_controller.isGrounded) moveDirection.y = 0;
        else
        {
            moveDirection.y += gravity.y * Time.deltaTime;
        }
    }

    Vector2 moveInput = Vector2.zero;
    Vector2 lookInput = Vector2.zero;
    private void HandleInput()
    {
        if (_canPickup)
        {
            TryPickupObject();
        }

        if (!_canPickup)
        {
            _pickuptimer += Time.deltaTime;
            if (_pickuptimer >= _timeToPickup)
            {
                _pickuptimer = 0f;
                Debug.Log("You can pick things up again");
                _canPickup = true;
            }
        }

        if (_heldObject != null)
        {
            if (_interact)
            {

                _throwTimer += Time.deltaTime;
                if (_throwTimer >= _timeToFullThrowForce)
                {
                    Debug.Log("Fully charged");
                    //_throwTimer = _timeToFullThrowForce;
                    ThrowObject();
                    _throwTimer = 0f;
                }

                DrawThrowTrajectoryInGameView();
            }
            if (!_interact && _throwTimer > _minimumInput)
            {
                
                Debug.Log("You're throwing");

                ThrowObject();
                _throwTimer = 0f;
            }
        }
    }

    Vector3 moveDirection = Vector3.zero;
    private void HandleMovement(float vertical, float horizontal)
    {
        //nothig happens if no input
        if (Mathf.Abs(horizontal) < _minimumInput
            && Mathf.Abs(vertical) < _minimumInput) moveDirection = Vector3.zero;

        moveDirection = _cameraForward * vertical + _cameraRight * horizontal;
    }

    private void ApplyMovement(Vector3 moveDirection)
    {
        _controller.Move(moveDirection * Time.deltaTime * _moveSpeed);
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
            if (collider.attachedRigidbody != null && _heldObject == null)
            {
                if (collider.attachedRigidbody.isKinematic)
                {
                    //Debug.Log("kinematic");
                    continue;
                }

                if (collider.GetComponent<ToppingHandler>().PlayerIndex != _playerInput.playerIndex)
                {
                    continue;
                }

                _heldObject = collider.attachedRigidbody;

                HoldObject(true);

                _heldObject.transform.SetParent(transform);

                _heldObject.transform.localPosition = new Vector3(0, 1, 1);

                _canPickup = false;

                break;
            }
        }
    }

    private void ThrowObject()
    {
        HoldObject(false);

        _heldObject.transform.SetParent(null);

        velocity = new Vector3(transform.forward.x, 0.5f, transform.forward.z).normalized * CalculateThrowForce();

        _heldObject.AddForce(velocity, ForceMode.Impulse);

        _heldObject = null;
        _canPickup = false;
    }
    private void HoldObject(bool v)
    {
        _heldObject.useGravity = !v;
        _heldObject.isKinematic = v;
    }

    private float CalculateThrowForce()
    {
        return _throwForce * (_throwTimer / _timeToFullThrowForce);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + lookDirection * 4);

        if (_heldObject != null)
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

        Vector3[] points = new Vector3[_trajectoryResolution];

        startPosition = _heldObject.position;
        velocity = new Vector3(transform.forward.x, 0.5f, transform.forward.z).normalized * CalculateThrowForce();

        for (int i = 0; i < _trajectoryResolution; i++)
        {
            float time = i * gizmoTimeStep;
            Vector3 position = startPosition + velocity * time + 0.5f * gravity * (time * time);
            if(position.y < -2) position.y = -2;
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

            startPosition = _heldObject.position;

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

