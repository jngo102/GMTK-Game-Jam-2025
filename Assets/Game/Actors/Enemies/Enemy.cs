using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int scoreOnDeath = 10;
    
    public string enemyType = "Enemy";
    [SerializeField] private Health health;
    [SerializeField] private Chaser chaser;
    [SerializeField] private DeathManager death;
    [SerializeField] private Attacker attacker;
    
    public void Awake()
    {
        var player = GameObject.FindWithTag("Player");

        chaser.target = player;
        attacker.target = player.transform;

        var scoreboard = FindAnyObjectByType<Scoreboard>(FindObjectsInactive.Include);
        death.Died.AddListener(() => scoreboard.AddScore(scoreOnDeath));
    }
}
