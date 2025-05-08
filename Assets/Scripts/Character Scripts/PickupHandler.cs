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
    private float _minimumThrowTime = 0.1f;
    [SerializeField]
    private LineRenderer _lineRenderer;
    [SerializeField]
    private int _trajectoryResolution = 30;

    private PlayerInput _playerInput;
    private GameObject _heldTopping;
    private Rigidbody _heldToppingBody;
    private float _pickupTimer;
    private float _throwTimer = 0f;
    [SerializeField]
    private bool _canPickup = true;

    private Vector3 _gravity = new Vector3(0, -9.81f, 0);

    private void Awake()
    {
        _playerInput = GetComponent<PlayerInput>();
    }

    public bool IsHolding => _heldTopping != null;

    private void Update()
    {

        Debug.Log(_heldTopping);
        if (_heldTopping == null && !GetComponent<CombatHandler>().IsHit)
        {
            TryPickupObject();
        }

        if (!_canPickup && _heldTopping == null)
        {
            _pickupTimer += Time.deltaTime;
            if (_pickupTimer >= _timeToPickup)
            {
                Debug.Log("Can Pick up again");
                _pickupTimer = 0f;
                _canPickup = true;
            }
        }
    }

    //private void TryAutoPickup()
    //{
       
    //    Collider[] colliders = Physics.OverlapSphere(transform.position, _pickupDistance);
    //    foreach (Collider collider in colliders)
    //    {
    //        ToppingHandler topping = collider.GetComponent<ToppingHandler>();
    //        if (topping == null || !topping.CanBePickedUp || topping.PlayerIndex != _playerInput.playerIndex) continue;

    //        _heldTopping = collider.gameObject;
    //        _heldToppingBody = _heldTopping.GetComponent<Rigidbody>();
    //        _heldTopping.transform.SetParent(transform);
    //        _heldTopping.transform.localPosition = new Vector3(0, 1, 1);

    //        HoldObject(true);

    //        _canPickup = false;
    //        break;
    //    }
    //}

    public void ChargeThrow( bool interact)
    {
        if (_heldTopping == null) return;

        if (interact)
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
        if (!interact && _throwTimer > _minimumThrowTime)
        {
            _canPickup = false;
            ThrowObject();
            _throwTimer = 0f;
            
        }
    }

    //private void ThrowObject()
    //{
    //    HoldObject(false);
    //    _heldTopping.transform.SetParent(null);
    //    Vector3 objectVelocity = (transform.forward + Vector3.up * 0.5f).normalized * CalculateThrowForce();

    //    _heldToppingBody.AddForce(objectVelocity, ForceMode.Impulse);

    //    _heldTopping = null;
    //    _canPickup = false;
    //}

    //private void HoldObject(bool isHeld)
    //{
    //    if (_heldToppingBody != null)
    //    {
    //        _heldToppingBody.useGravity = !isHeld;
    //        _heldToppingBody.isKinematic = isHeld;
    //    }
    //}

    //private float CalculateThrowForce()
    //{
    //    return _throwForce * (_throwTimer / _timeToFullThrowForce);
    //}

    Vector3 startPosition = Vector3.zero;
    Vector3 velocityOfTopping = Vector3.zero;
    float gizmoTimeStep = 0.1f;

    private void DrawThrowTrajectoryInGameView()
    {
        if (_lineRenderer == null)
            return; //use line renderer pls

        //there's an akward nullException when the object is being thrown automatically
        //now we simply tell this function not to do anything if there is no heldobject
        if (_heldTopping == null)
            return;

        Vector3[] points = new Vector3[_trajectoryResolution];

        startPosition = _heldTopping.transform.position;
        velocityOfTopping = new Vector3(transform.forward.x, 0.5f, transform.forward.z).normalized * CalculateThrowForce();

        for (int i = 0; i < _trajectoryResolution; i++)
        {
            float time = i * gizmoTimeStep;
            Vector3 position = startPosition + velocityOfTopping * time + 0.5f * Physics.gravity * (time * time);
            if (position.y < -2) position.y = -2;
            points[i] = position;
        }

        _lineRenderer.positionCount = _trajectoryResolution;
        _lineRenderer.SetPositions(points);
    }

    private void TryPickupObject()
    {
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
        _heldTopping = obj;
        _heldTopping.transform.SetParent(transform);
        _heldToppingBody = obj.GetComponent<Rigidbody>();
        _heldToppingBody.transform.localPosition = new Vector3(0, 1, 1);
    }

    Vector3 objectVelocity = Vector3.zero;

    private void ThrowObject()
    {
        HoldObject(false);
        
        _heldTopping.transform.SetParent(null);
        
        objectVelocity = new Vector3(transform.forward.x, 0.5f, transform.forward.z).normalized * CalculateThrowForce();

        _heldToppingBody.AddForce(objectVelocity, ForceMode.Impulse);

        _heldTopping = null;
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
}
