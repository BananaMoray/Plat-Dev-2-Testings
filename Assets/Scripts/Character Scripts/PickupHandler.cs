using UnityEngine;
using UnityEngine.InputSystem;

public class PickupHandler : MonoBehaviour
{
    [SerializeField]
    private float _pickupDistance = 2f; // Pickup distance
    [SerializeField]
    private float _timeToPickup = 1.5f;
    [SerializeField]
    private float _throwForce = 15f;
    [SerializeField]
    private float _timeToFullThrowForce = 2f;
    [SerializeField]
    private LineRenderer _lineRenderer;
    [SerializeField]
    private int _trajectoryResolution = 30;

    private PlayerInput _playerInput;
    private GameObject _heldTopping;
    private Rigidbody _heldToppingBody;
    private float _pickupTimer = 0f;
    private float _throwTimer = 0f;
    private bool _canPickup = true;

    private Vector3 _gravity = new Vector3(0, -9.81f, 0);

    private void Awake()
    {
        _playerInput = GetComponent<PlayerInput>();
    }

    public bool IsHolding => _heldTopping != null;

    private void Update()
    {
        // Check for automatic pickup if no object is currently being held
        if (_heldTopping == null)
        {
            TryAutoPickup();
        }

        // Handle the hold or throw logic if holding an object
        if (_heldTopping != null)
        {
            DrawThrowTrajectory();
        }

        // Handle the pickup cooldown timer
        if (!_canPickup && _heldTopping == null)
        {
            _pickupTimer += Time.deltaTime;
            if (_pickupTimer >= _timeToPickup)
            {
                _pickupTimer = 0f;
                _canPickup = true;
            }
        }
    }

    private void TryAutoPickup()
    {
        // Check for nearby toppings to automatically pick up
        Collider[] colliders = Physics.OverlapSphere(transform.position, _pickupDistance);
        foreach (Collider collider in colliders)
        {
            ToppingHandler topping = collider.GetComponent<ToppingHandler>();
            if (topping == null || !topping.CanBePickedUp || topping.PlayerIndex != _playerInput.playerIndex) continue;

            _heldTopping = collider.gameObject;
            _heldToppingBody = _heldTopping.GetComponent<Rigidbody>();
            _heldTopping.transform.SetParent(transform);
            _heldTopping.transform.localPosition = new Vector3(0, 1, 1);
            HoldObject(true);
            _canPickup = false;
            break;
        }
    }

    public void ChargeThrow()
    {
        if (_heldTopping == null) return;

        _throwTimer += Time.deltaTime;
        if (_throwTimer >= _timeToFullThrowForce)
        {
            ThrowObject();
            _throwTimer = 0f;
        }
    }

    private void ThrowObject()
    {
        HoldObject(false);
        _heldTopping.transform.SetParent(null);

        Vector3 objectVelocity = (transform.forward + Vector3.up * 0.5f).normalized * CalculateThrowForce();
        _heldToppingBody.AddForce(objectVelocity, ForceMode.Impulse);

        _heldTopping = null;
        _canPickup = false;
    }

    private void HoldObject(bool isHeld)
    {
        if (_heldToppingBody != null)
        {
            _heldToppingBody.useGravity = !isHeld;
            _heldToppingBody.isKinematic = isHeld;
        }
    }

    private float CalculateThrowForce()
    {
        return _throwForce * (_throwTimer / _timeToFullThrowForce);
    }

    private void DrawThrowTrajectory()
    {
        Vector3[] points = new Vector3[_trajectoryResolution];
        Vector3 startPosition = _heldTopping.transform.position;
        Vector3 velocity = (transform.forward + Vector3.up * 0.5f).normalized * CalculateThrowForce();

        for (int i = 0; i < _trajectoryResolution; i++)
        {
            float time = i * 0.1f;
            points[i] = startPosition + velocity * time + 0.5f * Physics.gravity * (time * time);
            if (points[i].y < -2) points[i].y = -2;
        }

        _lineRenderer.positionCount = _trajectoryResolution;
        _lineRenderer.SetPositions(points);
    }
}
