using UnityEngine;
using UnityEngine.EventSystems;

public class MenuButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Animator animator;
    [SerializeField] private string fmodHoverEvent;

    public void OnPointerEnter(PointerEventData eventData)
    {
        FMODUnity.RuntimeManager.PlayOneShot(fmodHoverEvent, transform.position);
        animator.Play("Open");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        animator.Play("Close");
    }
}
