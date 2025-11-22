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
    public float eliteInterval = 25f;
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

    private float timeSinceElite = 0f;

    // Активные группы волн
    private readonly List<Transform> activeGroups = new();

    void Start()
    {
        SceneManager.SetActiveScene(gameObject.scene);
    }

    void Update()
    {
        timeAlive += Time.deltaTime;
        timeSinceLastBoss += Time.deltaTime;
        timeSinceElite += Time.deltaTime;

        // Если босс активен — ждём
        if (bossActiveBlock)
        {
            timeSinceBossSpawned += Time.deltaTime;
            if (timeSinceBossSpawned >= bossBlockTime)
                bossActiveBlock = false;
        }

        // --- THREAT влияет на интервал волн ---
        float tf = threat.GetThreatFactor();

        float dynamicDelay = baseDelay * Mathf.Lerp(1f, 0.4f, tf);

        float spawnDelay = Mathf.Clamp(
            dynamicDelay - timeAlive * difficultyGrowth,
            minDelay,
            baseDelay
        );

        // Если верхняя группа мешает — ждём
        if (ActiveGroupsBlockingSpawn())
            return;

        if (!bossActiveBlock && Time.time >= nextWaveTime)
        {
            // Проверка на босса
            if (timeSinceLastBoss >= bossInterval)
            {
                SpawnBossWave();
                return;
            }

            // Проверка на элиту по таймеру
            if (timeSinceElite >= eliteInterval)
            {
                SpawnEliteWave();
                return;
            }

            // Обычная волна (весовая)
            SpawnWeightedRandomWave();
            nextWaveTime = Time.time + spawnDelay;
        }
    }

    // ---------------------------------------
    //   ПРОВЕРКА ГРУПП
    // ---------------------------------------

    bool ActiveGroupsBlockingSpawn()
    {
        if (activeGroups.Count == 0)
            return false;

        float camY = Camera.main.transform.position.y;
        float limit = camY + Camera.main.orthographicSize - 1f;

        foreach (var g in activeGroups)
        {
            if (g == null) continue;
            foreach (Transform c in g)
                if (c != null && c.position.y > limit)
                    return true;
        }

        activeGroups.RemoveAll(g => g == null);
        return false;
    }

    // ---------------------------------------
    //   ВОЛНЫ С УЧЁТОМ THREAT
    // ---------------------------------------

    void SpawnWeightedRandomWave()
    {
        float tf = threat.GetThreatFactor();

        // Чем выше TF — тем больше редких волн
        float rareBoost = Mathf.Lerp(0f, 0.15f, tf);

        float roll = Random.value;

        float dynamicEliteChance = eliteChance + tf * 0.25f;

        // Шанс внезапной элиты на обычной волне
        if (Random.value < dynamicEliteChance && elitePatterns.Count > 0)
        {
            SpawnPattern(elitePatterns);
            return;
        }

        if (roll < 0.60f - rareBoost)
        {
            SpawnPattern(normalPatterns);
            threat.ReduceThreat(3);             // ослабляем угрозу
        }
        else if (roll < 0.90f - rareBoost)
        {
            SpawnPattern(elitePatterns);
            threat.AddThreat(7);
        }
        else
        {
            SpawnPattern(rarePatterns);
            threat.AddThreat(12);
        }
    }

    void SpawnEliteWave()
    {
        timeSinceElite = 0f;
        threat.AddThreat(10);
        SpawnPattern(elitePatterns);
    }

    void SpawnPattern(List<WavePattern> list)
    {
        if (list.Count == 0)
            return;

        int index = Random.Range(0, list.Count);
        WavePattern chosen = list[index];

        Transform group = chosen.Spawn(this);
        if (group != null)
            activeGroups.Add(group);
    }

    // ---------------------------------------
    //              БОССЫ
    // ---------------------------------------

    void SpawnBossWave()
    {
        Debug.Log("SPAWNING BOSS WAVE");

        timeSinceLastBoss = 0f;
        timeSinceBossSpawned = 0f;
        bossActiveBlock = true;

        threat.AddThreat(25);

        if (bossPrefabs.Count == 0) return;

        int i = Random.Range(0, bossPrefabs.Count);
        Instantiate(bossPrefabs[i], bossSpawnPoint.position, Quaternion.identity);
    }

    // Для паттернов
    public void RegisterGroup(Transform g)
    {
        activeGroups.Add(g);
    }
}
