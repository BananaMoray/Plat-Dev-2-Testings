using System;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEditor.Searcher.SearcherWindow.Alignment;
using UnityEngine.InputSystem.XR;
using UnityEditor;
using Unity.VisualScripting;

public class CharacterControl : MonoBehaviour
{

    private CharacterController _controller;
    private PlayerInput _input;
    private Camera _camera;

    [SerializeField]
    private float _minimumInput = 0.1f;

    [SerializeField]
    private float _moveSpeed = 10f;
    [SerializeField]
    private float _rotationSpeed = 720f;
    [SerializeField]
    private float _throwForce = 10f;

    [SerializeField]
    private Material[] _playerColours;


    private Vector3 _cameraForward;
    private Vector3 _cameraRight;

    private Rigidbody _heldObject;

    private bool _canLift;

    private float _pickupDistance = 2f;


    //input system stuff
    private Vector2 _movementInput = Vector2.zero;
    private Vector2 _lookInput = Vector2.zero;
    private bool _interact;
    private bool _fire;

    private void Start()
    {
        _controller = GetComponent<CharacterController>();
        _input = GetComponent<PlayerInput>();
        _camera = Camera.main;
        CalculateCameraDirections();

        HandlePlayerColour(_input.playerIndex);
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
        CalculateMovement(_movementInput.y, _movementInput.x);
        HandleRotation(_lookInput.y, _lookInput.x);
        HandleGravity();
        ApplyMovement(moveDirection);
    }

    float gravity = -9.81f;
    private void HandleGravity()
    {
        if (_controller.isGrounded) moveDirection.y = 0;
        else 
        {
            moveDirection.y += gravity * Time.deltaTime;
        }
    }

    Vector2 moveInput = Vector2.zero;
    Vector2 lookInput = Vector2.zero;
    private void HandleInput()
    {

        if (_interact)
        {
            TryPickupObject();
        }
        if (_heldObject != null)
        {
            if (!_interact)
            {
                ThrowObject();
            }
        }

        //if (Gamepad.current != null)
        //{
        //    //moveInput = Gamepad.current.leftStick.ReadValue();
        //    //lookInput = Gamepad.current.rightStick.ReadValue();
        //    if (Gamepad.current.rightTrigger.wasReleasedThisFrame) ThrowObject();
        //    if (Gamepad.current.rightTrigger.wasPressedThisFrame) TryPickupObject();

        if (Gamepad.current != null)
            if (Gamepad.current.leftShoulder.wasReleasedThisFrame) Debug.Log(_input.playerIndex);

        //}
        //else if (Keyboard.current != null)
        //{
        //    //moveInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        //    //lookInput = new Vector2(Input.GetAxis("AltHorizontal"), Input.GetAxis("AltVertical"));

        //    if (Input.GetKeyDown(KeyCode.Space)) ThrowObject();
        //}

        //float horizontal = Input.GetAxis("Horizontal");
        //float vertical = Input.GetAxis("Vertical");

        //float lookHorizontal = Input.GetAxis("AltHorizontal");
        //float lookVertical = Input.GetAxis("AltVertical");


        //for some reason the lookinput is reversed, sooooo eeeeeeeermm
        //okay so, i was actually just pretty stupid
        //i placed the glasses on the back of the character :skull:


    }

    Vector3 moveDirection = Vector3.zero;
    private void CalculateMovement(float vertical, float horizontal)
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

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + lookDirection * 4);
        //Gizmos.DrawSphere(transform.position, _pickupDistance);
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
                //Debug.Log("pickup");
                _heldObject = collider.attachedRigidbody;
                _heldObject.useGravity = false;
                _heldObject.isKinematic = true;
                _heldObject.transform.SetParent(transform);
                _heldObject.transform.localPosition = new Vector3(0, 1, 1);
                break;
            }
        }
    }

    private void ThrowObject()
    {
        if (_heldObject != null)
        {
            _heldObject.useGravity = true;
            _heldObject.isKinematic = false; 
            _heldObject.transform.SetParent(null);

            //CalculateThrowForce();

            Vector3 throwDirection = new Vector3(transform.forward.x, 0.5f, transform.forward.z).normalized;

            _heldObject.AddForce(throwDirection * _throwForce, ForceMode.Impulse);
            _heldObject = null;
        }
    }

    private void CalculateThrowForce()
    {
        throw new NotImplementedException();
    }
}
