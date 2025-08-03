using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

[CreateAssetMenu(fileName = "Wave", menuName = "ScriptableObjects/Wave")]
public class Wave : ScriptableObject
{
    public float waveSpawnInterval;
    public int totalEnemies;
    public Enemy[] enemyChoices;
    public int[] maxEnemiesOfType;

    public int[] spawnChances;

    private int enemiesSpawned;

    private int enemiesRemaining;

    private Transform player;

    public UnityEvent WaveOver;
    
    public void Init()
    {
        enemiesSpawned = 0;
        enemiesRemaining = totalEnemies;
    }

    public void CreateNewEnemy(Vector2 position)
    {
        if (enemiesSpawned >= totalEnemies)
        {
            return;
        }
        List<Enemy> choices = new();
        for (var i = 0; i < enemyChoices.Length; i++)
        {
            var choice = enemyChoices[i];
            for (var j = 0; j < spawnChances[i]; j++)
            {
                choices.Add(choice);   
            }
        }
        var enemyChoice = choices[Random.Range(0, choices.Count)];
        var enemy = Instantiate(enemyChoice, position, Quaternion.identity);
        enemiesSpawned++;
        var death = enemy.GetComponentInChildren<DeathManager>();
        death.Died.AddListener(() =>
        {
            enemiesRemaining--;
            if (enemiesRemaining == 0)
            {
                WaveOver?.Invoke();
            }
        });
    }
}