using System.Collections;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(Canvas))]
public class LootMessage : MonoBehaviour
{
    private Animator animator;
    private Canvas canvas;
    [SerializeField] private TextMeshPro text;
    [SerializeField] private float showTime = 1;
    [SerializeField] private float showHeight = 64;

    private float showTimer;
    
    private void Awake()
    {
        canvas = GetComponentInParent<Canvas>();
        
        canvas.worldCamera = Camera.main;
    }

    public void Show(string message)
    {
        IEnumerator ShowRoutine()
        {
            transform.SetParent(null);
            text.text = message;

            while (showTimer < showTime)
            {
                showTimer += Time.deltaTime;
                var textColor = text.color;
                text.color = new Color(textColor.r, textColor.g, textColor.b, 1f - showTimer / showTime);
                text.transform.position += Vector3.up * (showHeight * Time.deltaTime);
                yield return null;
            }
            
            Destroy(gameObject);
        }
        

        StartCoroutine(ShowRoutine());
;    }
}
