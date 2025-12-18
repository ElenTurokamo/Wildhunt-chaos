using UnityEngine;
using System.Collections.Generic;

public class PlayerHealthUI : MonoBehaviour
{
    [Header("Список сердец")]
    public List<HeartUIElement> hearts; 

    private PlayerHealth playerHealth;

    void Start()
    {
        playerHealth = FindFirstObjectByType<PlayerHealth>();

        if (playerHealth != null)
        {
            UpdateHealthUI(playerHealth.maxLives);
        }
        
        PlayerHealth.OnLivesChanged += UpdateHealthUI;
    }
    
    void OnDestroy()
    {
        PlayerHealth.OnLivesChanged -= UpdateHealthUI;
    }

    private void UpdateHealthUI(int currentLives)
    {
        for (int i = 0; i < hearts.Count; i++)
        {
            if (i < currentLives)
            {
                hearts[i].SetFull();
            }
            else if (i == currentLives)
            {
                hearts[i].BreakHeart();
            }
            else
            {
                hearts[i].SetEmpty();
            }
        }
    }
}