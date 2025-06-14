using UnityEngine;
using UnityEngine.InputSystem;

public class PickupHandler : MonoBehaviour
{
    [SerializeField, Tooltip("Enabled only allows the pickup of toppings of the same ID as the player. Disable to pick up all toppings regardless of ID.")]
    public bool _canPickUpOnlyIDToppings = true;

    [Header("Pick Up Values")]
    [SerializeField]
    private float _pickupDistance = 2f;
    [SerializeField]
    private float _timeToPickup = 1.5f;
    [SerializeField]
    private Vector3 _toppingOffset = new Vector3(0, 1, 1);
    [SerializeField]
    private float _throwAngle = 0.5f;

    [Header("Throw Values")]
    [SerializeField]
    private float _throwForce = 15f;
    [SerializeField]
    private float _minThrowForce = 3f;
    [SerializeField]
    private float _timeToFullThrowForce = 2f;
    [SerializeField]
    private float _minimumThrowTime = 0.1f;

    [Header("Line Renderer Data")]
    [SerializeField]
    private LineRenderer _lineRenderer;
    [SerializeField]
    private int _trajectoryResolution = 30;

    private PlayerInput _playerInput;
    private CombatHandler _combatHandler;

    private GameObject _heldTopping;
    private Rigidbody _heldToppingBody;
    private float _toppingMass = 2f;
    private float _pickupTimer = 0f;
    private float _throwTimer = 0f;

    private bool _canPickup = true;



    private Vector3 _gravity = new Vector3(0, -9.81f, 0);

    public bool IsHolding => _heldTopping != null;

    public bool IsThrowing;

    private Animator _anim;

    private void Awake()
    {
        _playerInput = GetComponent<PlayerInput>();
        _combatHandler = GetComponent<CombatHandler>();
        _anim = GetComponent<Animator>();
    }

    private void Update()
    {
        _anim.SetBool("IsHolding", IsHolding);

        HandleBlock();

        if (_canPickup && _heldTopping == null && !_combatHandler.IsHit)
        {
            TryPickupObject();
        }

        if (!_canPickup && _heldTopping == null)
        {
            _pickupTimer += Time.deltaTime;
            if (_pickupTimer >= _timeToPickup)
            {
                _canPickup = true;
                _pickupTimer = 0f;
                //Debug.Log("Can pick up again");
            }
        }
    }

    public void ChargeThrow(bool interact)
    {
        if (_heldTopping == null) return;
        //IsThrowing = true;
        if (interact)
        {

            _lineRenderer.enabled = true;
            // Set the LineRenderer color to green
            if (gameObject.GetComponent<CharacterManager>().PlayerIndex == 0)
                _lineRenderer.material.color = Color.green;
            if (gameObject.GetComponent<CharacterManager>().PlayerIndex == 1)
                _lineRenderer.material.color = Color.white;
            if (gameObject.GetComponent<CharacterManager>().PlayerIndex == 2)
                _lineRenderer.material.color = Color.red;
            if (gameObject.GetComponent<CharacterManager>().PlayerIndex == 3)
                _lineRenderer.material.color = Color.yellow;
            DrawThrowTrajectoryInGameView();
            IsThrowing = true;
            _throwTimer += Time.deltaTime;
            if (_throwTimer >= _timeToFullThrowForce)
            {
                ThrowObject();
            }



        }
        else if (_throwTimer > _minimumThrowTime)
        {
            ThrowObject();
            IsThrowing = false;
        }

    }

    private void TryPickupObject()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, _pickupDistance);
        foreach (Collider collider in colliders)
        {
            ToppingHandler topping = collider.GetComponent<ToppingHandler>();

            if (topping == null || !topping.CanBePickedUp || topping.IsPickedUp) continue;

            if (_canPickUpOnlyIDToppings && topping.PlayerIndex != _playerInput.playerIndex) continue;

            AssignHeldObject(collider.gameObject);
            HoldObject(true);
            //Debug.Log("Picked up Ingredient");

            StartPickupCooldown();
            break;
        }
    }

    private void AssignHeldObject(GameObject obj)
    {
        _heldTopping = obj;
        _heldTopping.transform.SetParent(transform);
        _heldToppingBody = obj.GetComponent<Rigidbody>();
        _toppingMass = _heldToppingBody.mass;
        _heldToppingBody.transform.localPosition = _toppingOffset;
    }

    private void ThrowObject()
    {
        _lineRenderer.enabled = false;
        HoldObject(false);
        _heldTopping.transform.SetParent(null);

        Vector3 throwVelocity = new Vector3(transform.forward.x, _throwAngle, transform.forward.z).normalized * CalculateThrowForce();
        _heldToppingBody.AddForce(throwVelocity, ForceMode.Impulse);

        _heldTopping = null;
        _throwTimer = 0f;

        StartPickupCooldown();
    }

    public void DropObject()
    {
        if (_heldTopping == null) return;

        ThrowObject();
    }

    private void HoldObject(bool v)
    {
        _heldTopping.GetComponent<ToppingHandler>().IsPickedUp = v;
        _heldToppingBody.useGravity = !v;
        _heldToppingBody.isKinematic = v;
    }

    private float CalculateThrowForce()
    {

        return _minThrowForce + _throwForce * Mathf.Clamp01(_throwTimer / _timeToFullThrowForce);
    }

    private void DrawThrowTrajectoryInGameView()
    {

        if (_lineRenderer == null || _heldTopping == null) return;
        Vector3[] points = new Vector3[_trajectoryResolution];
        Vector3 startPosition = _heldTopping.transform.position;
        Vector3 velocity = new Vector3(transform.forward.x, _throwAngle, transform.forward.z).normalized * CalculateThrowForce();

        float timeStep = 0.1f;
        for (int i = 0; i < _trajectoryResolution; i++)
        {
            float time = i * timeStep;
            Vector3 position = startPosition + velocity / _toppingMass * time + 0.5f * Physics.gravity * time * time;
            points[i] = position;
        }

        _lineRenderer.positionCount = _trajectoryResolution;
        _lineRenderer.SetPositions(points);

    }

    private void HandleBlock()
    {
        if (_combatHandler.IsBlocking)
        {
            _lineRenderer.enabled = false;
            if (_heldTopping != null)
                _heldTopping.transform.localRotation = Quaternion.Euler(90f, 0, 0);
            _throwTimer = 0f;
        }
        else
        {
            if (_heldTopping != null)
                _heldTopping.transform.localRotation = Quaternion.identity;
        }
    }

    private void StartPickupCooldown()
    {
        _canPickup = false;
        _pickupTimer = 0f;
    }
}