using UnityEngine;

public class HealthPickup : Loot
{
    public float healthAdd = 1;

    protected override string Message => $"Gained {healthAdd} health!";
    
    public override void Pickup()
    {
        base.Pickup();
        var player = GameObject.FindGameObjectWithTag("Player");
        var health = player.GetComponentInChildren<Health>();
        health.CurrentHealth += healthAdd;
    }
}
