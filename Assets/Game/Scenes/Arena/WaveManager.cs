using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Pool;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class WaveManager : MonoBehaviour
{
    public float minSpawnDistance = 0.5f;
    public float maxSpawnDistance = 4f;
    
    public int currentWave = -1;
    public Wave[] waves;

    public UnityEvent<int> ToNextWave;
    public UnityEvent AllWavesComplete;

    public Tilemap tileMap;

    private float waveSpawnTimer;

    private Camera mainCam;
    
    private Wave CurrentWave => waves[currentWave];

    private Bounds tileMapBounds;

    private void Awake()
    {
        NextWave();
        mainCam = Camera.main;
        tileMapBounds = tileMap.localBounds;
    }

    public void NextWave()
    {
        currentWave++;
        if (currentWave >= waves.Length)
        {
            AllWavesComplete?.Invoke();
            return;
        }
        
        CurrentWave.Init();

        CurrentWave.WaveOver.AddListener(NextWave);

        ToNextWave?.Invoke(currentWave);
    }

    public void Update()
    {
        if (currentWave >= waves.Length)
        {
            return;
        }
        
        waveSpawnTimer += Time.deltaTime;
        if (waveSpawnTimer >= CurrentWave.waveSpawnInterval)
        {
            waveSpawnTimer = 0;
            SpawnEnemy();
        }
    }

    public void SpawnEnemy()
    {
        var camX = GetRandomValue();
        float camY;
        if (Mathf.Abs(camX) <= 1.0f)
        {
            camY = GetRandomValueOutsideViewportBounds();
        }
        else
        {
            camY = GetRandomValue();
        }

        var spawnPosition = KeepSpawnPointWithinTileMapBounds(new Vector2(camX, camY));
        
        CurrentWave.CreateNewEnemy(spawnPosition);
    }

    private float GetRandomValue()
    {
        return mainCam.orthographicSize * Random.Range(-Random.Range(minSpawnDistance, maxSpawnDistance),
            1 + Random.Range(minSpawnDistance, maxSpawnDistance));
    }

    private float GetRandomValueOutsideViewportBounds()
    {
        var valueChoices = new[]
        {
            -Random.Range(minSpawnDistance, maxSpawnDistance), Random.Range(minSpawnDistance, maxSpawnDistance)
        };
        return mainCam.orthographicSize * valueChoices[Random.Range(0, valueChoices.Length)];
    }

    private Vector2 KeepSpawnPointWithinTileMapBounds(Vector2 normalizedPosition)
    {
        var worldPosition = mainCam.ViewportToWorldPoint(normalizedPosition);
        bool atEdge = false;
        if (worldPosition.x < tileMapBounds.min.x)
        {
            normalizedPosition.x = 1 + mainCam.orthographicSize * Random.Range(minSpawnDistance, maxSpawnDistance);
            atEdge = true;
        }
        else if (worldPosition.x > tileMapBounds.max.x)
        {
            normalizedPosition.x = -mainCam.orthographicSize * Random.Range(minSpawnDistance, maxSpawnDistance);
            atEdge = true;
        }
        if (worldPosition.y < tileMapBounds.min.y)
        {
            normalizedPosition.y += 1 + mainCam.orthographicSize * Random.Range(minSpawnDistance, maxSpawnDistance);
            atEdge = true;
        }
        else if (worldPosition.y > tileMapBounds.max.y)
        {
            normalizedPosition.y = -mainCam.orthographicSize * Random.Range(minSpawnDistance, maxSpawnDistance);
            atEdge = true;
        }

        if (atEdge)
        {
            worldPosition = mainCam.ViewportToWorldPoint(normalizedPosition);
        }

        return worldPosition;
    }
}
