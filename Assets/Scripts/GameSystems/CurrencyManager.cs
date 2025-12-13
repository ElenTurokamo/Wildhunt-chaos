using UnityEngine;
using TMPro;

public class CurrencyManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TMP_Text currencyText;

    [Header("Настройки")]
    public int CurrentCurrency { get; private set; } = 0;

    [Header("Эффекты")]
    [SerializeField] private float pulseScale = 1.2f;
    [SerializeField] private float pulseDuration = 0.2f;
    private Color defaultColor;

    void Start()
    {
        if (currencyText != null)
            defaultColor = currencyText.color;

        ResetCurrency();
    }

    public void AddCurrency(int amount)
    {
        CurrentCurrency += amount;
        UpdateUI();
        
        if (currencyText != null)
            StartCoroutine(PulseEffect());
    }

    public void ResetCurrency()
    {
        CurrentCurrency = 0;
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (currencyText != null)
            currencyText.text = CurrentCurrency.ToString();
    }

    private System.Collections.IEnumerator PulseEffect()
    {
        float timer = 0f;
        Vector3 originalScale = Vector3.one;
        Vector3 targetScale = Vector3.one * pulseScale;

        while (timer < pulseDuration)
        {
            timer += Time.deltaTime;
            float t = timer / pulseDuration;
            
            // Простой пинг-понг эффект: увеличение и возврат
            if (t < 0.5f)
                currencyText.transform.localScale = Vector3.Lerp(originalScale, targetScale, t * 2);
            else
                currencyText.transform.localScale = Vector3.Lerp(targetScale, originalScale, (t - 0.5f) * 2);

            yield return null;
        }
        currencyText.transform.localScale = originalScale;
    }
}