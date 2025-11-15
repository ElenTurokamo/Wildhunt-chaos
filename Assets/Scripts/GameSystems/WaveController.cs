using UnityEngine;
using System.Collections.Generic;

public class WaveController : MonoBehaviour
{
    [Header("Паттерны волн")]
    public List<WavePattern> normalPatterns;
    public List<WavePattern> elitePatterns;
    public List<WavePattern> rarePatterns;

    [Header("Тайминги волн")]
    public float baseDelay = 5f;
    public float minDelay = 1.2f;
    public float difficultyGrowth = 0.03f;

    [Header("Боссы")]
    public GameObject bossPrefab;
    public Transform bossSpawnPoint;
    public float bossInterval = 60f;

    [Header("Система угрозы")]
    public ThreatSystem threat;

    private float nextWaveTime;
    private float timeAlive = 0f;
    private float timeSinceLastBoss = 0f;

    void Update()
    {
        timeAlive += Time.deltaTime;
        timeSinceLastBoss += Time.deltaTime;

        float spawnDelay = Mathf.Clamp(
            baseDelay - timeAlive * difficultyGrowth,
            minDelay,
            baseDelay
        );

        if (Time.time >= nextWaveTime)
        {
            threat.ResetThreat();
            
            if (timeSinceLastBoss >= bossInterval)
            {
                SpawnBossWave();
            }
            else
            {
                SpawnWeightedRandomWave();
            }

            nextWaveTime = Time.time + spawnDelay;
        }
    }

    // ----------------------
    //      ВЫБОР ВОЛНЫ
    // ----------------------

    void SpawnWeightedRandomWave()
    {
        float roll = Random.value;

        if (roll < 0.60f)
            SpawnFromList(normalPatterns);

        else if (roll < 0.90f)
            SpawnFromList(elitePatterns);

        else
            SpawnFromList(rarePatterns);
    }

    void SpawnBossWave()
    {
        Debug.Log("SPAWNING BOSS WAVE");

        Instantiate(bossPrefab, bossSpawnPoint.position, Quaternion.identity);
        timeSinceLastBoss = 0f;
    }

    // ----------------------
    //      СПАВН ВОЛН
    // ----------------------

    void SpawnFromList(List<WavePattern> list)
    {
        if (list.Count == 0)
            return;

        int index = Random.Range(0, list.Count);
        WavePattern chosen = list[index];
        chosen.Spawn(this);
    }
}
