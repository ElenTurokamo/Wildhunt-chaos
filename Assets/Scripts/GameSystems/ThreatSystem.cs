using UnityEngine;

public class ThreatSystem : MonoBehaviour
{
    [Header("Параметры угрозы")]
    public float baseThreat = 15f;           // стартовая "емкость" волны
    public float growthRate = 0.5f;          // насколько быстро растет количество угрозы в секунду

    private float timeAlive = 0f;
    private float maxThreat = 0f;
    private float currentThreat = 0f;

    void Update()
    {
        timeAlive += Time.deltaTime;

        // Максимальная угроза волны растет со временем
        maxThreat = baseThreat + timeAlive * growthRate;

        // Перед каждой волной угрозу нужно сбрасывать полностью
        // но сбрасывать вручную должен WaveController
        // (он решает, когда новая волна)
    }

    // Вызывается WaveController перед созданием новой волны
    public void ResetThreat()
    {
        currentThreat = maxThreat;
    }

    // Проверяет, можно ли потратить угрозу
    // Если можно — тратит и возвращает true
    public bool TrySpend(int cost)
    {
        if (currentThreat - cost < 0)
            return false;

        currentThreat -= cost;
        return true;
    }

    // Чтобы паттерны могли проверять "сколько угрозы осталось"
    public float GetRemainingThreat()
    {
        return currentThreat;
    }

    // Debug (чтобы было приятно смотреть в инспекторе)
    private void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 300, 40), $"Threat: {currentThreat:F1}/{maxThreat:F1}");
    }
}
