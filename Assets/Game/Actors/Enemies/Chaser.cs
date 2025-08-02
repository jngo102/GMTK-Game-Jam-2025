using UnityEngine;

public class Chaser : MonoBehaviour
{
    public GameObject target;

    [SerializeField] private Mover mover;

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

    private void OnDisable()
    {
        mover.Stop();
    }
}