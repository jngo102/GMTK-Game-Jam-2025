using System;
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

    private void Update()
    {
        if (gameActions.HasValue)
        {
            var moveVector = gameActions.Value.Move.ReadValue<Vector2>();
            if (moveVector.magnitude > 0)
            {
                mover.Move(moveVector);
            }
            else
            {
                mover.Stop();
            }
        }
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
        }
    }
}