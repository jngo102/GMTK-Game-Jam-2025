using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class PlayerInputHandler : MonoBehaviour, ISaveable
{
    private PlayerInput playerInput;

    private PlayerInputActions referencePlayerActions =  new();

    public bool Enabled
    {
        get => playerInput.enabled;
        set => playerInput.enabled = value;
    }

    private void Awake()
    {
        SetupInputActions();
    }

    private void SetupInputActions()
    {
        playerInput = GetComponent<PlayerInput>();
        var overridesJson = referencePlayerActions.asset.SaveBindingOverridesAsJson();
        playerInput.actions.LoadBindingOverridesFromJson(overridesJson);
    }

    private void OnEnable()
    {
        Enable();
    }

    public void Enable()
    {
        playerInput.actions.Enable();
    }

    private void OnDisable()
    {
        Disable();
    }

    public void Disable()
    {
        playerInput.actions.Disable();
    }

    public void LoadData(SaveData saveData)
    {
        if (string.IsNullOrEmpty(saveData.bindingOverrides)) return;
        
        referencePlayerActions.asset.LoadBindingOverridesFromJson(saveData.bindingOverrides);               
    }

    public void SaveData(SaveData saveData)
    {
        saveData.bindingOverrides = referencePlayerActions.asset.SaveBindingOverridesAsJson();
    }
}
