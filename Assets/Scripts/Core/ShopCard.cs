using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class ShopCard : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image bgImage;
    [SerializeField] private Image iconImage;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text descText;
    [SerializeField] private TMP_Text priceText;
    [SerializeField] private Button buyButton;

    [Header("Shake Settings")]
    [SerializeField] private float shakeDuration = 0.3f;
    [SerializeField] private float shakeStrength = 10f;

    private ShopItemData _data;
    private ShopManager _manager;
    private RectTransform _btnRect;
    private Vector2 _originalBtnPos;

    public void Setup(ShopItemData data, ShopManager manager, Sprite bgSprite, Color titleColor)
    {
        _data = data;
        _manager = manager;

        nameText.text = data.itemName;
        nameText.color = titleColor;    

        descText.text = data.description;
        priceText.text = $"{data.price}";
        iconImage.sprite = data.icon;
        
        if (bgSprite != null) 
            bgImage.sprite = bgSprite;

        buyButton.onClick.RemoveAllListeners();
        buyButton.onClick.AddListener(OnBuyClick);

        _btnRect = buyButton.GetComponent<RectTransform>();
        _originalBtnPos = _btnRect.anchoredPosition;
    }

    private void OnBuyClick()
    {
        bool success = _manager.TryPurchaseItem(_data);

        if (!success)
        {
            StopAllCoroutines();
            StartCoroutine(ShakeButton());
        }
    }

    private IEnumerator ShakeButton()
    {
        float elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            float x = Mathf.Sin(elapsed * 50f) * shakeStrength;
            _btnRect.anchoredPosition = _originalBtnPos + new Vector2(x, 0);
            
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        _btnRect.anchoredPosition = _originalBtnPos;
    }
}