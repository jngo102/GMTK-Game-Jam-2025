using UnityEngine.InputSystem;

public class InputManager : Singleton<InputManager>, ISaveable
{
    public PlayerInputActions PlayerActions { get; private set; }

    protected override void OnAwake()
    {
        PlayerActions = new PlayerInputActions();
    }

    public void LoadData(SaveData saveData)
    {
        PlayerActions.asset.LoadBindingOverridesFromJson(saveData.bindingOverrides);
    }

    public void SaveData(SaveData saveData)
    {
        saveData.bindingOverrides = PlayerActions.asset.SaveBindingOverridesAsJson();
    }
}