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

    void Update()
    {
        timeAlive += Time.deltaTime;

        // формула, которая делает maxThreat плавно растущей
        maxThreat = baseThreat
                  + (timeAlive * growthRate)
                  + Mathf.Min(reserveCap, difficultyBonus);
    }

    // -----------------------------------------
    //                 ВОЛНА
    // -----------------------------------------
    public void ResetThreat()
    {
        // устанавливаем угрозу на максимум + бонусы сложности
        currentThreat = maxThreat;
        spentThisWave = 0;
    }

    public void CompleteWave()
    {
        float usedPercent = spentThisWave / maxThreat;

        // если волна была тяжёлой — увеличим бонус сложности
        if (usedPercent >= 0.8f)
        {
            difficultyBonus += maxThreat * difficultyMultiplier;
        }
        else if (usedPercent < 0.4f)
        {
            // слишком лёгкая — уменьшаем бонус
            difficultyBonus *= 0.6f;
        }

        difficultyBonus = Mathf.Clamp(difficultyBonus, 0, reserveCap);
    }

    public void Refund(float amount)
    {
        currentThreat = Mathf.Min(currentThreat + amount, maxThreat);
    }

    // -----------------------------------------
    //                 РАСХОД УГРОЗЫ
    // -----------------------------------------
    public bool TrySpend(int cost)
    {
        // хватает обычной угрозы
        if (currentThreat >= cost)
        {
            currentThreat -= cost;
            spentThisWave += cost;
            return true;
        }

        // если долг разрешён и можно его взять
        if (allowDebt)
        {
            float debtNeeded = cost - currentThreat;

            if (Mathf.Abs(currentThreat - cost) <= maxDebt)
            {
                currentThreat = currentThreat - cost; // уйдём в минус
                spentThisWave += cost;
                return true;
            }
        }

        // не смогли заплатить
        return false;
    }

    public float GetRemainingThreat() => currentThreat;

    public float GetSpent() => spentThisWave;

    public float GetMaxThreat() => maxThreat;

    // -----------------------------------------
    //                 DEBUG
    // -----------------------------------------
    private void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 300, 80),
            $"Threat: {currentThreat:F1}/{maxThreat:F1}\n" +
            $"Spent: {spentThisWave:F1}\n" +
            $"DifficultyBonus: {difficultyBonus:F1}");
    }

    // -----------------------------------------
    //        ДОПОЛНИТЕЛЬНЫЕ МЕТОДЫ
    // -----------------------------------------

    // Нормированное значение угрозы (0..1)
    public float GetThreatFactor()
    {
        if (maxThreat <= 0f) return 0f;
        float clamped = Mathf.Clamp01((maxThreat - currentThreat) / maxThreat);
        return clamped; // чем меньше осталось — тем выше TF
    }

    // Уменьшить угрозу (как штраф/успокоение)
    public void ReduceThreat(float amount)
    {
        currentThreat = Mathf.Clamp(currentThreat - amount, -maxDebt, maxThreat);
    }

    // Увеличить угрозу (при тяжёлых волнах)
    public void AddThreat(float amount)
    {
        currentThreat = Mathf.Clamp(currentThreat + amount, -maxDebt, maxThreat);
    }

}

