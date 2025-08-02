using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class RangeDetector : MonoBehaviour
{
    public UnityEvent InRange;
    
    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            InRange?.Invoke();
        }
    }
}
