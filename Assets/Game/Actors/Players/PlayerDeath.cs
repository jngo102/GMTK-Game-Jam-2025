using UnityEngine;

public class PlayerDeath : MonoBehaviour
{
    public Transform actor;
    public DeathManager death;
    
    private void Awake()
    {
        death.Died.AddListener(OnPlayerDeath);
    }

    private void OnPlayerDeath()
    {
        foreach (var component in actor.GetComponentsInChildren<Component>())
        {
            if (component is Animator animator)
            {
                animator.Play("Death");
                continue;
            }
            if (component is MonoBehaviour behavior)
            {
                behavior.enabled = false;
            }
        }
    }
}
