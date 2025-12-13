using UnityEngine;

public class ThreatSystem : MonoBehaviour
{
    [Header("Основные параметры")]
    public float baseThreat = 10f;         // минимальная угроза любой волны
    public float growthRate = 0.4f;        // рост угрозы со временем
    public float reserveCap = 25f;         // максимум дополнительной угрозы
    public float difficultyMultiplier = 0.05f; // усиление от успешных волн

    [Header("Долг / редкие события")]
    public bool allowDebt = true;          // можно ли уходить в минус
    public float maxDebt = 8f;             // не даём уйти в глубокий минус

    private float timeAlive = 0f;

    private float maxThreat = 0f;          // потолок угрозы на текущую волну   
    private float currentThreat = 0f;      // сколько сейчас осталось
    private float spentThisWave = 0f;      // сколько потратили в этой волне

    private float difficultyBonus = 0f;    // бонус за предыдущие волны

    // --- FIX 3: Инициализация при старте ---
    void Start()
    {
        // Сразу рассчитываем maxThreat и даем бюджет, чтобы враги пошли с 0-й секунды
        CalculateMaxThreat();
        ResetThreat();
    }

    void Update()
    {
        timeAlive += Time.deltaTime;
        CalculateMaxThreat();
    }

    void CalculateMaxThreat()
    {
        // формула, которая делает maxThreat плавно растущей
        maxThreat = baseThreat
                  + (timeAlive * growthRate)
                  + Mathf.Min(reserveCap, difficultyBonus);
    }

    public float GetTimeAlive() => timeAlive;


    // -----------------------------------------
    //                 ВОЛНА
    // -----------------------------------------
    public void ResetThreat()
    {
        // устанавливаем угрозу на максимум + бонусы сложности
        // Это позволяет "перезаряжать" бюджет перед каждой новой пачкой врагов
        currentThreat = maxThreat;
        spentThisWave = 0;
    }

    public void CompleteWave()
    {
        // Логику можно оставить, если вы планируете вызывать её вручную,
        // но для непрерывного спавна важнее рост maxThreat от времени.
        float usedPercent = maxThreat > 0 ? spentThisWave / maxThreat : 0;

        if (usedPercent >= 0.8f)
            difficultyBonus += maxThreat * difficultyMultiplier;
        else if (usedPercent < 0.4f)
            difficultyBonus *= 0.6f;

        difficultyBonus = Mathf.Clamp(difficultyBonus, 0, reserveCap);
    }

    public void Refund(float amount)
    {
        currentThreat = Mathf.Min(currentThreat + amount, maxThreat);
    }

    // -----------------------------------------
    //                РАСХОД УГРОЗЫ
    // -----------------------------------------
    public bool TrySpend(int cost)
    {
        if (currentThreat >= cost)
        {
            currentThreat -= cost;
            spentThisWave += cost;
            return true;
        }

        if (allowDebt)
        {
            // Исправлена проверка: если текущий threat 5, а cost 10, abs(5-10) = 5. Если maxDebt 8, то ок.
            if (Mathf.Abs(currentThreat - cost) <= maxDebt)
            {
                currentThreat -= cost; 
                spentThisWave += cost;
                return true;
            }
        }

        return false;
    }

    public float GetRemainingThreat() => currentThreat;
    public float GetSpent() => spentThisWave;
    public float GetMaxThreat() => maxThreat;

    // -----------------------------------------
    //                 DEBUG (ВКЛ/ВЫКЛ)
    // -----------------------------------------
    // private void OnGUI()
    // {
    //     GUI.Label(new Rect(10, 10, 300, 80),
    //         $"Threat: {currentThreat:F1}/{maxThreat:F1}\n" +
    //         $"Spent: {spentThisWave:F1}\n" +
    //         $"DiffBonus: {difficultyBonus:F1}\n" +
    //         $"TimeAlive: {timeAlive:F1}");
    // }

    // -----------------------------------------
    //          ДОПОЛНИТЕЛЬНЫЕ МЕТОДЫ
    // -----------------------------------------
    public float GetThreatFactor()
    {
        if (maxThreat <= 0f) return 0f;
        // Здесь логика инвертирована: если угрозы мало (потратили), фактор растет?
        // Или если макс угроза растет? Оставим как было, но добавим защиту от деления на 0.
        float clamped = Mathf.Clamp01((maxThreat - currentThreat) / maxThreat);
        return clamped; 
    }

    public void ReduceThreat(float amount)
    {
        currentThreat = Mathf.Clamp(currentThreat - amount, -maxDebt, maxThreat);
    }

    public void AddThreat(float amount)
    {
        currentThreat = Mathf.Clamp(currentThreat + amount, -maxDebt, maxThreat);
    }
}