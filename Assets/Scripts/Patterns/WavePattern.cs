using UnityEngine;

public abstract class WavePattern : ScriptableObject
{
    [Header("Базовые настройки паттерна")]
    public string patternName;
    
    [Tooltip("Префаб 1")]
    public GameObject enemyPrefab;
    
    [Tooltip("Префаб 2")]
    public GameObject enemyPrefabAlt; 
    
    public int threatCost = 1; 

    public abstract Transform Spawn(WaveController controller);
}