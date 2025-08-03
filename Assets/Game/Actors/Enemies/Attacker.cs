using System;
using UnityEngine;
using UnityEngine.Events;

public class Attacker : MonoBehaviour
{
    private static readonly int attackParam = Animator.StringToHash("Attack");
    
    public GameObject attackPrefab;

    public Transform attackOffset;

    public Animator animator;

    public bool parentAttackToActor;

    public float attackAngle;

    public string attackAnim;

    public Mover mover;

    private GameObject currentAttack;

    public UnityEvent AttackEnded;

    public Transform target;
    
    public string fmodAttackEvent;

    private void OnDisable()
    {
        DestroyAttack();
    }

    public void Attack()
    {
        if (animator.IsPlaying(attackAnim))
        {
            return;
        }
        
        var diff = target.position - transform.position;
        attackAngle = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
        
        animator.Play(attackAnim);
    }

    public void CreateAttack()
    {
        currentAttack = Instantiate(attackPrefab, attackOffset.transform.position, Quaternion.identity);
        currentAttack.transform.rotation = Quaternion.Euler(0, 0, attackAngle);
        if (parentAttackToActor)
        {
            currentAttack.transform.SetParent(attackOffset);
        }
        
        FMODUnity.RuntimeManager.PlayOneShot(fmodAttackEvent, transform.position);
    }

    public void DestroyAttack()
    {
        if (currentAttack)
        {
            Destroy(currentAttack);
        }
    }

    public void EndAttack()
    {
        AttackEnded?.Invoke();
    }
}
