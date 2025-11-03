using UnityEngine;

public class BackgroundScroller : MonoBehaviour
{
    [System.Serializable]
    public class Layer
    {
        public Transform background1; // основной (будет установлен в (0,0))
        public Transform background2; // копия (будет установленa в (0, spriteHeight))
        public float scrollSpeed = 1f;
        [HideInInspector] public float spriteHeight;
    }

    public Layer[] layers;

    void Start()
    {
        foreach (var layer in layers)
        {
            if (layer.background1 == null || layer.background2 == null)
            {
                Debug.LogWarning("BackgroundScroller: один из background'ов не задан в слое.");
                continue;
            }

            // Получаем высоту в мировых единицах (учитывает масштаб)
            SpriteRenderer sr = layer.background1.GetComponent<SpriteRenderer>();
            if (sr == null)
            {
                Debug.LogError("BackgroundScroller: background1 не содержит SpriteRenderer.");
                continue;
            }

            layer.spriteHeight = sr.bounds.size.y;

            // Ставим background1 ровно в мировую позицию (0,0) (оставляем z как был)
            layer.background1.position = new Vector3(0f, 0f, layer.background1.position.z);

            // Ставим background2 ровно над ним (чтобы отрисовка с самого старта была бесшовной)
            layer.background2.position = new Vector3(
                0f,
                layer.background1.position.y + layer.spriteHeight,
                layer.background2.position.z
            );
        }
    }

    void Update()
    {
        foreach (var layer in layers)
        {
            if (layer.background1 == null || layer.background2 == null) continue;

            // Двигаем оба слоя вниз одинаково (мировые координаты)
            Vector3 delta = Vector3.down * layer.scrollSpeed * Time.deltaTime;
            layer.background1.position += delta;
            layer.background2.position += delta;

            // Когда один полностью уходит вниз (ниже -spriteHeight), перемещаем его ровно над другим
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
