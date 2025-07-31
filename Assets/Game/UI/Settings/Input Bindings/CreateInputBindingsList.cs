using System.Collections.Generic;
using UnityEngine;

public class CreateInputBindingsList : MonoBehaviour
{
    [SerializeField] private RebindUI rebindUIPrefab;

    private List<RebindUI> rebindUIs = new();

    void Awake()
    {
        foreach (var action in InputManager.Instance.PlayerActions.Game.Get().actions)
        {
            var rebindUI = Instantiate(rebindUIPrefab.gameObject, transform).GetComponent<RebindUI>();
            rebindUI.name = $"{action.name} Rebinder";
            rebindUI.actionName = action.name;
            rebindUI.Rebound += OnInputRebound;
            rebindUIs.Add(rebindUI);
        }
    }

    private void OnInputRebound(string actionName, string inputPath)
    {
        foreach (var rebindUI in rebindUIs)
        {
            if (rebindUI.actionName == actionName)
            {
                continue;
            }
            rebindUI.CheckDuplicateBinding(inputPath);
        }
    }
}