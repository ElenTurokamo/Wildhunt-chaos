using UnityEngine;

public class CurrencyPickup : MonoBehaviour
{
    [Header("Настройки")]
    public int amount = 1;
    public string currencyPickupSoundName = "Currency_Pickup-1";

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (AudioManager.instance != null)
            {
                AudioManager.instance.PlaySFXOneShot(currencyPickupSoundName);
            }
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