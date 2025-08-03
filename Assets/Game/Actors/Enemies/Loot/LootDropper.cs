using System.Collections.Generic;
using UnityEngine;

public class LootDropper : MonoBehaviour
{
    public Loot[] possibleLoot;

    public float lootChance;

    public int[] lootDropWeights;

    public DeathManager death;

    public void Awake()
    {
        death.Died.AddListener(CheckDropLoot);
    }

    private void CheckDropLoot()
    {
        var value = Random.Range(0f, 1f);
        if (value >= lootChance)
        {
            return;
        }
        
        var loots = new List<Loot>();
        for (var i = 0; i < lootDropWeights.Length; i++)
        {
            var currentLoot = possibleLoot[i];
            var currentWeight = lootDropWeights[i];

            for (var j = 0; j < currentWeight; j++)
            {
                loots.Add(currentLoot);
            }
            var loot = loots[Random.Range(0, loots.Count)];
            SpawnLoot(loot);
        }
    }

    private void SpawnLoot(Loot loot)
    {
        Instantiate(loot, transform.position, Quaternion.identity);
    }
}
