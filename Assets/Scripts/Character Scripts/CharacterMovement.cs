using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] 
    private float _groundDrag = 6f;
    [SerializeField] 
    private float _acceleration = 3f;
    [SerializeField] 
    private float _moveSpeedMax = 10f;
    [SerializeField] 
    private float _moveSpeedSlowMax = 4f;
    [SerializeField] 
    private float _rotationSpeed = 720f;
    [SerializeField] 
    private float _minimumInput = 0.1f;

    private CharacterController _controller;
    private Camera _mainCamera;

    private Vector3 _cameraForward;
    private Vector3 _cameraRight;

    public Vector3 Velocity;
    private float _verticalVelocity;
    private Vector3 _gravity = new Vector3(0, -9.81f, 0);

    private Vector2 _movementInput;
    private Vector2 _lookInput;
    public bool IsHoldingObject { get; set; }

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
        _mainCamera = Camera.main;
        CalculateCameraDirections();
    }

    private void CalculateCameraDirections()
    {
        _cameraForward = _mainCamera.transform.forward;
        _cameraRight = _mainCamera.transform.right;
        _cameraForward.y = 0;
        _cameraRight.y = 0;
        _cameraForward.Normalize();
        _cameraRight.Normalize();
    }

    public void SetInputs(Vector2 moveInput, Vector2 lookInput)
    {
        _movementInput = moveInput;
        _lookInput = lookInput;
    }
    

    private void Update()
    {
        HandleGravity();
        HandleMovement();
        HandleRotation();
        ApplyVelocity();
    }

    private void HandleGravity()
    {
        if (_controller.isGrounded)
            _verticalVelocity = -1f;
        else
            _verticalVelocity += _gravity.y * Time.deltaTime;

        Velocity.y = _verticalVelocity;
    }

    private void HandleMovement()
    {
        Vector3 inputDir = Vector3.zero;
        if (Mathf.Abs(_movementInput.x) >= _minimumInput || Mathf.Abs(_movementInput.y) >= _minimumInput)
            inputDir = (_cameraForward * _movementInput.y + _cameraRight * _movementInput.x).normalized;

        Velocity += inputDir * _acceleration;

        if (_controller.isGrounded)
            Velocity *= (1 - Time.deltaTime * _groundDrag);

        float tempY = Velocity.y;
        Velocity.y = 0;
        //if holding an object ? returns _moveSpeedSlowMax, if not returns _moveSpeedMax
        Velocity = Vector3.ClampMagnitude(Velocity, IsHoldingObject ? _moveSpeedSlowMax : _moveSpeedMax);
        Velocity.y = tempY;
    }

    private void HandleRotation()
    {
        Vector3 direction = (_cameraForward * _lookInput.y + _cameraRight * _lookInput.x);
        if (direction.sqrMagnitude < 0.01f)
            direction = (_cameraForward * _movementInput.y + _cameraRight * _movementInput.x);

        if (direction.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, _rotationSpeed * Time.deltaTime);
        }
    }

    private void ApplyVelocity()
    {
        _controller.Move(Velocity * Time.deltaTime);
    }
}
