using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Mover))]
public class PlayerInputHandler : MonoBehaviour
{
    /// <summary>
    ///     An instance of player actions for input rebinding.
    /// </summary>
    private PlayerInputActions.GameActions? gameActions {
        get
        {
            if (!InputManager.Instance)
            {
                return null;
            }
            return InputManager.Instance.PlayerActions.Game;           
        }
}

    public bool Enabled
    {
        get
        {
            if (!gameActions.HasValue)
            {
                return false;
            }
            return gameActions.Value.enabled;
        }
        set
        {
            if (value)
            {
                Enable();
            }
            else
            {
                Disable();
            }
        }
    }

    private Mover mover;

    private void Awake()
    {
        mover = GetComponent<Mover>();
    }

    private void OnEnable()
    {
        Enable();
    }

    public void Enable()
    {
        if (gameActions.HasValue)
        {
            gameActions.Value.Enable();
            gameActions.Value.Move.performed += OnMove;
            gameActions.Value.Move.canceled += OnStop;    
        }
    }

    private void OnDisable()
    {
        Disable();
    }

    public void Disable()
    {
        if (gameActions.HasValue)
        {
            gameActions.Value.Disable();
            gameActions.Value.Move.performed -= OnMove;
            gameActions.Value.Move.canceled -= OnStop;    
        }
    }

    private void OnMove(InputAction.CallbackContext ctx)
    {
        var vector = ctx.ReadValue<Vector2>();
        mover.Move(vector);
    }
    
    private void OnStop(InputAction.CallbackContext ctx)
    {
        mover.Stop();
    }
}