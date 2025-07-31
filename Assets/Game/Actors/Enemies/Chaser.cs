using UnityEngine;

[RequireComponent(typeof(Mover))]
public class Chaser : MonoBehaviour
{
    public GameObject target;

    private Mover mover;

    private void Awake()
    {
        mover = GetComponent<Mover>();
    }

    public void Update()
    {
        if (target)
        {
            var diff = (target.transform.position - transform.position).normalized;
            mover.Move(diff);
        }
    }
}