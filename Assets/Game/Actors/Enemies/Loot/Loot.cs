using TMPro;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Loot : MonoBehaviour
{
    protected virtual string Message => "Picked up loot!";

    [SerializeField] private LootMessage lootMessagePrefab;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Pickup();
            var lootMsgObj = Instantiate(lootMessagePrefab.gameObject, transform.position,  Quaternion.identity);
            var lootMessage = lootMsgObj.GetComponent<LootMessage>();
            lootMessage.Show(Message);
            Destroy(gameObject);
        }
    }

    public virtual void Pickup()
    {
        
    }
}
