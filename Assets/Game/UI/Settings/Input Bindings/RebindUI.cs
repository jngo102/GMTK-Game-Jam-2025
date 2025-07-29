using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
///     Handles the rebinding of a single input action.
/// </summary>
public class RebindUI : MonoBehaviour {
    /// <summary>
    ///     The name of the action.
    /// </summary>
    [SerializeField] private string actionName;

    /// <summary>
    ///     Whether the input action is composite.
    /// </summary>
    [SerializeField] private bool isComposite;

    /// <summary>
    ///     The name of the composite input action.
    /// </summary>
    [SerializeField] private string compositeName;

    /// <summary>
    ///     The text that displays the name of the action.
    /// </summary>
    [SerializeField] private TextMeshProUGUI actionLabel;

    /// <summary>
    ///     The text that displays the bound key or mouse button.
    /// </summary>
    [SerializeField] private TextMeshProUGUI keyMouseText;

    /// <summary>
    ///     The index of the input action in the action map's bindings.
    /// </summary>
    private int bindingIndex = -1;

    /// <summary>
    ///     The input action that is managed by this UI.
    /// </summary>
    private InputAction inputAction;

    private void Start() {
        inputAction = UIManager.Instance.ReferencePlayerActions.Player.Get().actions
            .First(action => action.name == actionName);
        if (isComposite)
            bindingIndex = inputAction.bindings.IndexOf(binding =>
                binding.isPartOfComposite && binding.name == compositeName.ToLower());
        else
            bindingIndex = inputAction.bindings.IndexOf(binding => !binding.isComposite);

        actionLabel.text = isComposite ? compositeName : actionName;
        keyMouseText.text = InputControlPath.ToHumanReadableString(
            inputAction.bindings[bindingIndex].effectivePath,
            InputControlPath.HumanReadableStringOptions.OmitDevice);
    }

    /// <summary>
    ///     Begin the rebinding process.
    /// </summary>
    public void StartRebind() {
        inputAction.Disable();
        actionLabel.text = "Listening...";
        inputAction.PerformInteractiveRebinding(bindingIndex)
            .OnMatchWaitForAnother(0.1f)
            .WithCancelingThrough("<Keyboard>/escape")
            .OnCancel(RebindCancel)
            .OnComplete(RebindComplete)
            .Start();
    }

    /// <summary>
    ///     Callback for when the rebinding process is canceled.
    /// </summary>
    /// <param name="operation">The rebinding operation object passed into the callback.</param>
    private void RebindCancel(InputActionRebindingExtensions.RebindingOperation operation) {
        actionLabel.text = InputControlPath.ToHumanReadableString(
            inputAction.bindings[bindingIndex].effectivePath,
            InputControlPath.HumanReadableStringOptions.OmitDevice);
        operation.Dispose();
        operation.action.Enable();
    }

    /// <summary>
    ///     Callback for when the rebinding process is complete.
    /// </summary>
    /// <param name="operation">The rebinding operation object passed into the callback.</param>
    private void RebindComplete(InputActionRebindingExtensions.RebindingOperation operation) {
        actionLabel.text = operation.selectedControl.displayName;
        operation.Dispose();
        operation.action.Enable();
    }
}