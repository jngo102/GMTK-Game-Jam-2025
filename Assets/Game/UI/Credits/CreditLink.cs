using UnityEngine;

public class CreditLink : MonoBehaviour
{
    [SerializeField] private string link;

    public void OpenLink()
    {
        if (string.IsNullOrEmpty(link))
        {
            return;
        }
        Application.OpenURL(link);
    }
}
