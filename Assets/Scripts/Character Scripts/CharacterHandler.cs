using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterHandler : MonoBehaviour
{
    [SerializeField]
    private Material[] _playerColours;

    private PlayerInput _playerInput;
    private CharacterMovement _movement;
    private CombatHandler _combat;
    private PickupHandler _pickup;

    private Vector2 _movementInput;
    private Vector2 _lookInput;
    private bool _interact;
    private bool _fire;
    private bool _pause;

    private void Awake()
    {
        _playerInput = GetComponent<PlayerInput>();
        _movement = GetComponent<CharacterMovement>();
        _combat = GetComponent<CombatHandler>();
        _pickup = GetComponent<PickupHandler>();
        HandlePlayerColour(_playerInput.playerIndex);
    }
    private void HandlePlayerColour(int playerIndex)
    {
        GetComponent<MeshRenderer>().material = _playerColours[playerIndex];
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

    public void ResetPlayer()
    {
        transform.SetParent(null);
        transform.rotation = Quaternion.identity;
        GetComponent<CharacterController>().enabled = true;

        _combat.IsHit = false;
    }

    private void Update()
    {
        if (_combat.IsHit) return;

        _movement.SetInputs(_movementInput, _lookInput);
        _movement.IsHoldingObject = _pickup.IsHolding;

        _combat.HandleAttack(_fire, _pickup.IsHolding);
        _pickup.ChargeThrow(_interact);

    }
}
