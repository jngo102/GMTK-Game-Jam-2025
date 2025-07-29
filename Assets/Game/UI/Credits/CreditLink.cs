using UnityEngine;

public class CreditLink : MonoBehaviour {
    /// <summary>
    ///     The link that will be opened when clicking on the credited person's profile.
    /// </summary>
    [SerializeField] private string link;

    /// <summary>
    ///     Open the link to the credited person's profile.
    /// </summary>
    public void OpenLink() {
        if (string.IsNullOrEmpty(link)) return;
        Application.OpenURL(link);
    }
}