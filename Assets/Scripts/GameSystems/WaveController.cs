using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class WaveController : MonoBehaviour
{
    [Header("Паттерны волн")]
    public List<WavePattern> normalPatterns;
    public List<WavePattern> elitePatterns;
    public List<WavePattern> rarePatterns;

    [Header("Боссы")]
    public List<GameObject> bossPrefabs;
    public Transform bossSpawnPoint;
    public float bossInterval = 60f;
    public float bossBlockTime = 20f;

    [Header("Элита")]
    public float eliteInterval = 15f; 
    public float eliteChance = 0.15f;

    [Header("Тайминги волн")]
    public float baseDelay = 5f;
    public float minDelay = 1.2f;
    public float difficultyGrowth = 0.03f;

    [Header("Система угрозы")]
    public ThreatSystem threat;

    private float nextWaveTime;
    private float timeAlive = 0f;

    private float timeSinceLastBoss = 0f;
    private float timeSinceBossSpawned = 0f;
    private bool bossActiveBlock = false;
    
    private GameObject currentBoss; 
    private float bossDefeatedTimer = 0f; 

    private float timeSinceElite = 0f;

    private WavePattern lastElitePattern;
    private WavePattern lastNormalPattern;
    private WavePattern lastRarePattern; 

    private readonly List<Transform> activeGroups = new();

    void Start()
    {
        SceneManager.SetActiveScene(gameObject.scene);
        nextWaveTime = Time.time + 1f; 
    }

    void Update()
    {
        timeAlive += Time.deltaTime;
        timeSinceLastBoss += Time.deltaTime;
        timeSinceElite += Time.deltaTime;

        if (bossActiveBlock)
        {
            if (currentBoss == null)
            {
                bossDefeatedTimer += Time.deltaTime;
                if (bossDefeatedTimer >= 3f)
                {
                    bossActiveBlock = false;
                    timeSinceLastBoss = 0f; 
                }
            }
            else
            {
                timeSinceBossSpawned += Time.deltaTime;
                bossDefeatedTimer = 0f; 
                if (timeSinceBossSpawned >= bossBlockTime)
                {
                    bossActiveBlock = false;
                    timeSinceLastBoss = 0f;
                }
            }
            return;
        }

        if (ActiveGroupsBlockingSpawn())
            return;

        if (Time.time >= nextWaveTime)
        {
            if (timeSinceLastBoss >= bossInterval)
            {
                SpawnBossWave();
                return;
            }

            if (timeSinceElite >= eliteInterval)
            {
                SpawnEliteWave(true);
                nextWaveTime = Time.time + CalculateDelay();
                return;
            }

            threat.ResetThreat();
            SpawnWeightedRandomWave();
            nextWaveTime = Time.time + CalculateDelay();
        }
    }

    float CalculateDelay()
    {
        float tf = threat.GetThreatFactor();
        float dynamicDelay = baseDelay * Mathf.Lerp(1f, 0.4f, tf);
        return Mathf.Clamp(dynamicDelay - (timeAlive * difficultyGrowth), minDelay, baseDelay);
    }

    bool ActiveGroupsBlockingSpawn()
    {
        if (activeGroups.Count == 0) return false;
        float camY = Camera.main.transform.position.y;
        float limit = camY + Camera.main.orthographicSize - 1f;

        foreach (var g in activeGroups)
        {
            if (g == null) continue;
            foreach (Transform c in g)
                if (c != null && c.position.y > limit) return true;
        }
        activeGroups.RemoveAll(g => g == null);
        return false;
    }

    void SpawnWeightedRandomWave()
    {
        float tf = threat.GetThreatFactor();
        float rareBoost = Mathf.Lerp(0f, 0.20f, tf);
        float roll = Random.value;

        if (elitePatterns.Count > 0 && Random.value < eliteChance)
        {
            SpawnEliteWave(false);
            return;
        }
        
        if (roll < 0.85f - rareBoost)
        {
            SpawnPattern(normalPatterns, ref lastNormalPattern);
        }
        else
        {
            SpawnPattern(rarePatterns, ref lastRarePattern); 
        }
    }

    void SpawnEliteWave(bool resetTimer)
    {
        if (resetTimer) timeSinceElite = 0f;
        threat.AddThreat(1000); 
        SpawnPattern(elitePatterns, ref lastElitePattern);
    }

    void SpawnPattern(List<WavePattern> list, ref WavePattern lastPattern)
    {
        if (list == null || list.Count == 0) return;

        WavePattern chosen = null;

        if (list.Count == 1)
        {
            chosen = list[0];
        }
        else
        {
            int attempts = 10;
            do
            {
                int index = Random.Range(0, list.Count);
                chosen = list[index];
                attempts--;
            } 
            while (chosen == lastPattern && attempts > 0);
        }

        if (chosen == null)
        {
            Debug.LogWarning("WaveController: Выбран пустой паттерн (null). Проверьте списки в Инспекторе на наличие пустых полей (None).");
            return; 
        }

        lastPattern = chosen;

        Transform group = chosen.Spawn(this);
        if (group != null)
            activeGroups.Add(group);
    }

    void SpawnBossWave()
    {
        Debug.Log("SPAWNING BOSS WAVE");

        bossActiveBlock = true;
        timeSinceBossSpawned = 0f;
        bossDefeatedTimer = 0f;

        threat.AddThreat(1000);
        
        if (bossPrefabs.Count == 0) return;

        int i = Random.Range(0, bossPrefabs.Count);
        if (bossPrefabs[i] != null) 
        {
            currentBoss = Instantiate(bossPrefabs[i], bossSpawnPoint.position, Quaternion.identity);
        }
    }

    public void RegisterGroup(Transform g)
    {
        activeGroups.Add(g);
    }

    public float GetTimeUntilElite()
    {
        return eliteInterval - timeSinceElite;  
    }
}