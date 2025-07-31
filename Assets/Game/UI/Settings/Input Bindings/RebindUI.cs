using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

/// <summary>
///     Handles the rebinding of a single input action.
/// </summary>
public class RebindUI : MonoBehaviour
{
    private const string UnboundString = "<Unbound>";

    /// <summary>
    ///     The name of the action.
    /// </summary>
    public string actionName;

    /// <summary>
    ///     The text that displays the name of the action.
    /// </summary>
    [SerializeField] private TextMeshProUGUI actionLabel;

    /// <summary>
    ///     The text that displays the bound key or mouse button.
    /// </summary>
    [SerializeField] private TextMeshProUGUI keyMouseText;

    /// <summary>
    ///     The text that displays the bound gamepad button.
    /// </summary>
    [SerializeField] private TextMeshProUGUI gamepadText;

    /// <summary>
    ///     The input action that is managed by this UI.
    /// </summary>
    private InputAction inputAction;

    /// <summary>
    ///     Event sent when a key/mouse button or gamepad button is rebound.
    /// </summary>
    public UnityAction<string, string> Rebound;

    private InputActionMap InputMap => InputManager.Instance.PlayerActions.Game.Get();

    private InputBinding? KeyMouseBinding
    {
        get
        {
            var bindingIndex = inputAction.GetBindingIndex(InputBinding.MaskByGroups("Keyboard", "Mouse"));
            if (bindingIndex < 0)
            {
                return null;
            }

            return inputAction.bindings[bindingIndex];
        }
    }

    private InputBinding? GamepadBinding
    {
        get
        {
            var bindingIndex = inputAction.GetBindingIndex(InputBinding.MaskByGroups("Gamepad"));
            if (bindingIndex < 0)
            {
                return null;
            }

            return inputAction.bindings[bindingIndex];
        }
    }

    private void Start()
    {
        inputAction = InputMap.actions.First(action => action.name == actionName);

        actionLabel.text = actionName;

        if (KeyMouseBinding.HasValue)
        {
            keyMouseText.text = InputControlPath.ToHumanReadableString(
                KeyMouseBinding.Value.effectivePath,
                InputControlPath.HumanReadableStringOptions.OmitDevice);
        }
        else
        {
            keyMouseText.text = UnboundString;
        }

        if (GamepadBinding.HasValue)
        {
            gamepadText.text = InputControlPath.ToHumanReadableString(
                GamepadBinding.Value.effectivePath,
                InputControlPath.HumanReadableStringOptions.OmitDevice);
        }
        else
        {
            gamepadText.text = UnboundString;
        }
    }

    /// <summary>
    ///     Begin the rebinding process.
    /// </summary>
    public void StartRebindingKeyMouse()
    {
        inputAction.Disable();
        keyMouseText.text = "Listening...";
        var bindingIndex = inputAction.GetBindingIndex(InputBinding.MaskByGroups("Keyboard", "Mouse"));
        inputAction.PerformInteractiveRebinding(bindingIndex)
            .OnMatchWaitForAnother(0.1f)
            .WithCancelingThrough("*/{Cancel}")
            .OnCancel(RebindCancelKeyMouse)
            .OnComplete(RebindCompleteKeyMouse)
            .Start();
    }

    /// <summary>
    ///     Begin the rebinding process.
    /// </summary>
    public void StartRebindingGamepad()
    {
        inputAction.Disable();
        gamepadText.text = "Listening...";
        var bindingIndex = inputAction.GetBindingIndex(InputBinding.MaskByGroups("Gamepad"));
        inputAction.PerformInteractiveRebinding(bindingIndex)
            .OnMatchWaitForAnother(0.1f)
            .WithCancelingThrough("*/{Cancel}")
            .OnCancel(RebindCancelGamepad)
            .OnComplete(RebindCompleteGamepad)
            .Start();
    }

    public void ClearKeyMouseBinding()
    {
        var bindingIndex = inputAction.GetBindingIndex(InputBinding.MaskByGroups("Keyboard", "Mouse"));
        inputAction.RemoveBindingOverride(bindingIndex);
        var bindingPath = inputAction.bindings[bindingIndex].effectivePath;
        keyMouseText.text = InputControlPath.ToHumanReadableString(bindingPath,
            InputControlPath.HumanReadableStringOptions.OmitDevice);
        Rebound?.Invoke(actionName, bindingPath);
    }

    public void ClearGamepadBinding()
    {
        var bindingIndex = inputAction.GetBindingIndex(InputBinding.MaskByGroups("Gamepad"));
        inputAction.RemoveBindingOverride(bindingIndex);
        var bindingPath = inputAction.bindings[bindingIndex].effectivePath;
        gamepadText.text = InputControlPath.ToHumanReadableString(bindingPath,
            InputControlPath.HumanReadableStringOptions.OmitDevice);
        Rebound?.Invoke(actionName, bindingPath);
    }

    public void CheckDuplicateBinding(string inputPath)
    {
        if (KeyMouseBinding.HasValue && KeyMouseBinding.Value.effectivePath == inputPath)
        {
            ClearKeyMouseBinding();
        }
        else if (GamepadBinding.HasValue && GamepadBinding.Value.effectivePath == inputPath)
        {
            ClearGamepadBinding();
        }
    }

    /// <summary>
    ///     Callback for when the rebinding process is canceled for a key/mouse listener.
    /// </summary>
    /// <param name="operation">The rebinding operation object passed into the callback.</param>
    private void RebindCancelKeyMouse(InputActionRebindingExtensions.RebindingOperation operation)
    {
        var action = operation.action;
        var bindingIndex = action.GetBindingIndex(InputBinding.MaskByGroups("Keyboard", "Mouse"));
        keyMouseText.text = InputControlPath.ToHumanReadableString(action.bindings[bindingIndex].effectivePath,
            InputControlPath.HumanReadableStringOptions.OmitDevice);
        operation.Dispose();
        operation.action.Enable();
    }

    /// <summary>
    ///     Callback for when the rebinding process is canceled for a gamepad listener.
    /// </summary>
    /// <param name="operation">The rebinding operation object passed into the callback.</param>
    private void RebindCancelGamepad(InputActionRebindingExtensions.RebindingOperation operation)
    {
        var action = operation.action;
        var bindingIndex = action.GetBindingIndex(InputBinding.MaskByGroups("Gamepad"));
        gamepadText.text = InputControlPath.ToHumanReadableString(action.bindings[bindingIndex].effectivePath,
            InputControlPath.HumanReadableStringOptions.OmitDevice);
        operation.Dispose();
        operation.action.Enable();
    }

    /// <summary>
    ///     Callback for when the rebinding process is complete for a key/mouse listener.
    /// </summary>
    /// <param name="operation">The rebinding operation object passed into the callback.</param>
    private void RebindCompleteKeyMouse(InputActionRebindingExtensions.RebindingOperation operation)
    {
        keyMouseText.text = operation.selectedControl.displayName;
        if (KeyMouseBinding.HasValue)
        {
            Rebound?.Invoke(actionName, KeyMouseBinding.Value.effectivePath);
        }

        CompleteRebind(operation);
    }

    /// <summary>
    ///     Callback for when the rebinding process is complete for a gamepad listener.
    /// </summary>
    /// <param name="operation">The rebinding operation object passed into the callback.</param>
    private void RebindCompleteGamepad(InputActionRebindingExtensions.RebindingOperation operation)
    {
        gamepadText.text = operation.selectedControl.displayName;
        if (GamepadBinding.HasValue)
        {
            Rebound?.Invoke(actionName, GamepadBinding.Value.effectivePath);
        }

        CompleteRebind(operation);
    }

    private void CompleteRebind(InputActionRebindingExtensions.RebindingOperation operation)
    {
        operation.Dispose();
        operation.action.Enable();
    }
}