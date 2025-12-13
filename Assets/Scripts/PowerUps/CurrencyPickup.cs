using UnityEngine;

public class CurrencyPickup : MonoBehaviour
{
    [Header("Настройки")]
    public int amount = 1;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Collect();
        }
    }

    private void Collect()
    {
        CurrencyManager manager = Object.FindFirstObjectByType<CurrencyManager>();
        
        if (manager != null)
        {
            manager.AddCurrency(amount);
        }
        else
        {
            Debug.LogWarning("CurrencyManager не найден в сцене!");
        }

        Destroy(gameObject);
    }
}