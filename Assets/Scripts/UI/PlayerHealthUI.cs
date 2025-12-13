using UnityEngine;
using System.Collections.Generic;

public class PlayerHealthUI : MonoBehaviour
{
    [Header("Список сердец")]
    // ВАЖНО: Заполните этот список в Inspector слева направо.
    // Элемент 0 = Левое сердце.
    // Элемент 2 = Правое сердце.
    public List<HeartUIElement> hearts; 

    private PlayerHealth playerHealth;

    void Start()
    {
        playerHealth = FindFirstObjectByType<PlayerHealth>();

        if (playerHealth != null)
        {
            // Инициализация (сразу рисуем все полные)
            UpdateHealthUI(playerHealth.maxLives);
        }
        
        // Подписка на событие
        PlayerHealth.OnLivesChanged += UpdateHealthUI;
    }
    
    void OnDestroy()
    {
        PlayerHealth.OnLivesChanged -= UpdateHealthUI;
    }

    private void UpdateHealthUI(int currentLives)
    {
        // Проходим по всем сердцам в списке
        for (int i = 0; i < hearts.Count; i++)
        {
            if (i < currentLives)
            {
                // Это сердце должно быть целым (жизней хватает на этот индекс)
                hearts[i].SetFull();
            }
            else if (i == currentLives)
            {
                // Логика "Справа Налево":
                // Если у нас было 3 жизни, а стало 2, то currentLives = 2.
                // Индекс 0 и 1 остаются целыми.
                // Индекс 2 (третье сердце, правое) должно разбиться.
                
                // Вызываем анимацию раскола для сердца, которое только что потеряли
                hearts[i].BreakHeart();
            }
            else
            {
                // Все сердца правее текущего расколотого должны быть пустыми
                // (например, если мы потеряли сразу 2 жизни)
                hearts[i].SetEmpty();
            }
        }
    }
}