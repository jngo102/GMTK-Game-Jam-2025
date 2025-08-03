using UnityEngine;

public class LootDropper : MonoBehaviour
{
    public Loot[] possibleLoot;

    public float[] lootChances;

    public DeathManager death;

    public void Awake()
    {
        death.Died.AddListener(CheckDropLoot);
    }

    private void CheckDropLoot()
    {
        for (var i = 0; i < lootChances.Length; i++)
        {
            var currentLoot = possibleLoot[i];
            var currentChance = lootChances[i];
            var value = Random.Range(0f, 1f);
            if (value < currentChance)
            {
                SpawnLoot(currentLoot);
                return;
            }
        }
    }

    private void SpawnLoot(Loot loot)
    {
        Instantiate(loot, transform.position, Quaternion.identity);
    }
}
