using UnityEngine;

public abstract class WavePattern : ScriptableObject
{
    [Header("Базовые настройки паттерна")]
    public string patternName;
    public GameObject enemyPrefab;
    public int threatCost = 1; 

    public abstract void Spawn(WaveController controller);
}
