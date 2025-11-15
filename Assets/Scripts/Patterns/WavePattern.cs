using UnityEngine;

public abstract class WavePattern : ScriptableObject
{
    [Header("Базовые настройки паттерна")]
    public string patternName;
    public GameObject enemyPrefab;
    public int threatCost = 1; 

    // Основной метод. Все наследники обязаны реализовать свою логику спавна.
    public abstract void Spawn(WaveController controller);
}
