using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HeartUIElement : MonoBehaviour
{
    [Header("Спрайты")]
    public Sprite fullHeartSprite;   // Целое сердце
    public Sprite brokenHeartSprite; // Расколотое (момент удара)
    public Sprite emptyHeartSprite;  // Пустое (после удара)

    [Header("Настройки дрожи")]
    [SerializeField] private float shakeMagnitude = 5f; // Амплитуда
    [SerializeField] private float shakeDuration = 0.5f; // Длительность

    private Image heartImage;
    private Vector3 originalPosition;
    private bool isBroken = false; // Чтобы не ломать уже сломанное

    void Awake()
    {
        heartImage = GetComponent<Image>();
        originalPosition = transform.localPosition;
    }

    public void SetFull()
    {
        isBroken = false;
        StopAllCoroutines();
        transform.localPosition = originalPosition; // Сброс позиции
        if (heartImage != null) heartImage.sprite = fullHeartSprite;
    }

    public void SetEmpty()
    {
        isBroken = true;
        StopAllCoroutines();
        transform.localPosition = originalPosition;
        if (heartImage != null) heartImage.sprite = emptyHeartSprite;
    }

    // Метод для запуска анимации раскола
    public void BreakHeart()
    {
        if (isBroken) return; // Если уже разбито, не повторяем
        isBroken = true;

        if (heartImage != null) heartImage.sprite = brokenHeartSprite;
        StartCoroutine(ShakeAndSettle());
    }

    private IEnumerator ShakeAndSettle()
    {
        float elapsed = 0f;
        
        while (elapsed < shakeDuration)
        {
            // Дрожь влево-вправо по оси X
            float xOffset = Random.Range(-shakeMagnitude, shakeMagnitude);
            transform.localPosition = originalPosition + new Vector3(xOffset, 0, 0);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Возвращаем на место и ставим "пустой" спрайт
        transform.localPosition = originalPosition;
        if (heartImage != null) heartImage.sprite = emptyHeartSprite;
    }
}