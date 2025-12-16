using UnityEngine;

public class DeathSpawner : MonoBehaviour
{
    [Header("Настройки посмертного спавна")]
    [Tooltip("Префаб объекта, который появится после смерти (например, турель)")]
    public GameObject objectToSpawn;

    [Range(0f, 100f)]
    [Tooltip("Шанс появления объекта в процентах")]
    public float spawnChance = 30f;

    [Tooltip("Время в секундах, через которое созданный объект сам уничтожится")]
    public float spawnedObjectLifetime = 5f;

    public void TrySpawnOnDeath()
    {
        if (objectToSpawn == null)
        {
            return;
        }

        if (Random.value * 100f <= spawnChance)
        {
            SpawnObject();
        }
    }

    private void SpawnObject()
    {
        GameObject spawnedInstance = Instantiate(objectToSpawn, transform.position, Quaternion.identity);

        Destroy(spawnedInstance, spawnedObjectLifetime);
    }
}