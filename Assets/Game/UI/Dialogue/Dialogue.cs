using UnityEngine;
using UnityEngine.Localization;

[CreateAssetMenu(fileName = "Dialogue", menuName = "ScriptableObjects/Dialogue", order = 1)]
public class Dialogue : ScriptableObject {
    public LocalizedString[] pages;
}