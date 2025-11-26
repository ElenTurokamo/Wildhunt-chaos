using UnityEngine;

public class BackgroundScroller : MonoBehaviour
{
    [System.Serializable]
    public class Theme
    {
        public string name; 
        public Sprite[] layerSprites;
        
        // --- НОВОЕ: Опциональный префаб (эффекты, партиклы, оверлей) ---
        [Tooltip("Опционально. Префаб (например, партиклы дождя), который заспавнится как дочерний объект.")]
        public GameObject optionalPrefab; 
    }

    [System.Serializable]
    public class Layer
    {
        public Transform background1;
        public Transform background2; 
        public float scrollSpeed = 1f;
        [HideInInspector] public float spriteHeight;
    }

    [Header("Настройки тем")]
    public Theme[] themes; 

    [Header("Настройки слоев")]
    public Layer[] layers; 

    void Start()
    {
        if (themes.Length > 0)
        {
            int randomIndex = Random.Range(0, themes.Length);
            Theme selectedTheme = themes[randomIndex];
            ApplyTheme(selectedTheme);
        }
        else
        {
            Debug.LogWarning("BackgroundScroller: Список тем пуст! Используются спрайты, уже назначенные на сцене.");
        }

        foreach (var layer in layers)
        {
            if (layer.background1 == null || layer.background2 == null)
            {
                Debug.LogWarning("BackgroundScroller: один из background'ов не задан в слое.");
                continue;
            }

            SpriteRenderer sr = layer.background1.GetComponent<SpriteRenderer>();
            if (sr == null)
            {
                Debug.LogError("BackgroundScroller: background1 не содержит SpriteRenderer.");
                continue;
            }

            layer.spriteHeight = sr.bounds.size.y;

            layer.background1.position = new Vector3(0f, 0f, layer.background1.position.z);

            layer.background2.position = new Vector3(
                0f,
                layer.background1.position.y + layer.spriteHeight,
                layer.background2.position.z
            );
        }
    }

    void ApplyTheme(Theme theme)
    {
        // 1. Применяем спрайты слоев
        for (int i = 0; i < layers.Length; i++)
        {
            if (i >= theme.layerSprites.Length)
            {
                Debug.LogError($"В теме '{theme.name}' не хватает спрайтов для слоя {i}!");
                continue;
            }

            Sprite spriteToUse = theme.layerSprites[i];

            if (spriteToUse == null) continue;

            var sr1 = layers[i].background1.GetComponent<SpriteRenderer>();
            var sr2 = layers[i].background2.GetComponent<SpriteRenderer>();

            if (sr1 != null) sr1.sprite = spriteToUse;
            if (sr2 != null) sr2.sprite = spriteToUse;
        }

        // --- НОВОЕ: Логика спавна опционального объекта ---
        if (theme.optionalPrefab != null)
        {
            // Создаем объект. 
            // Второй аргумент 'transform' делает его дочерним объектом того, на чем висит этот скрипт.
            GameObject createdObject = Instantiate(theme.optionalPrefab, transform);
            
            // Сбрасываем локальную позицию в (0,0,0), чтобы он встал ровно по центру родителя
            createdObject.transform.localPosition = Vector3.zero;
        }
    }

    void Update()
    {
        foreach (var layer in layers)
        {
            if (layer.background1 == null || layer.background2 == null) continue;

            Vector3 delta = Vector3.down * layer.scrollSpeed * Time.deltaTime;
            layer.background1.position += delta;
            layer.background2.position += delta;

            if (layer.background1.position.y <= -layer.spriteHeight)
                MoveToTop(layer.background1, layer.background2, layer.spriteHeight);

            if (layer.background2.position.y <= -layer.spriteHeight)
                MoveToTop(layer.background2, layer.background1, layer.spriteHeight);
        }
    }

    void MoveToTop(Transform moved, Transform other, float spriteHeight)
    {
        moved.position = new Vector3(
            other.position.x,
            other.position.y + spriteHeight,
            moved.position.z
        );
    }
}