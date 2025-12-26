using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

public class ShopManager : MonoBehaviour
{
    [Header("Dependencies")]
    public CurrencyManager currencyManager;
    public WaveController waveController;

    [Header("UI Elements")]
    public GameObject shopAlertUI;
    public Image alertTimerImg;
    public GameObject shopUI;
    public Animator alertAnimator;

    [Header("Shop Content")]
    [SerializeField] private Transform itemsContainer;
    [SerializeField] private ShopCard cardPrefab;
    [SerializeField] private List<ShopItemData> availableItems;
    [SerializeField] private int itemsToDisplayCount = 3;

    [Header("Background Styles")]
    [SerializeField] private Sprite healingBg;
    [SerializeField] private Sprite defenseBg;
    [SerializeField] private Sprite buffBg;

    [Header("Text Colors")]
    [SerializeField] private Color healingColor = Color.green;
    [SerializeField] private Color defenseColor = Color.blue;
    [SerializeField] private Color buffColor = Color.red;

    [Header("Settings")]
    public float initialDelay = 60f;
    public float shopCooldown = 120f;
    public float alertTimeWindow = 10f;
    public float closeAnimDuration = 1f;
    public KeyCode openKey = KeyCode.F;

    public bool IsShopOpen { get; private set; } = false;

    private float nextShopAvailableTime;
    private bool isAlertActive = false;
    private bool isClosing = false;
    private float currentAlertTimer = 0f;

    void Start()
    {
        shopAlertUI.SetActive(false);
        shopUI.SetActive(false);
        nextShopAvailableTime = Time.time + initialDelay;
    }

    void Update()
    {
        if (IsShopOpen) return;

        if (!isAlertActive && !isClosing)
        {
            if (Time.time >= nextShopAvailableTime)
            {
                ShowAlert();
            }
        }
        else if (isAlertActive && !isClosing)
        {
            UpdateAlertState();
            HandleAlertInput();
        }
    }

    public bool TryPurchaseItem(ShopItemData item)
    {
        if (currencyManager == null || item == null) return false;

        if (currencyManager.CurrentCurrency >= item.price)
        {
            currencyManager.AddCurrency(-item.price);
            ApplyItemEffect(item);
            return true;
        }
        return false;
    }

    private void ApplyItemEffect(ShopItemData item)
    {
        Debug.Log($"Предмет куплен: {item.itemName}");
    }

private void GenerateShopItems()
    {
        if (itemsContainer == null || availableItems == null || availableItems.Count == 0) return;

        foreach (Transform child in itemsContainer)
        {
            Destroy(child.gameObject);
        }

        List<ShopItemData> finalSelection = new List<ShopItemData>();

        var healingPool = availableItems.Where(i => i.itemType == ItemType.Healing).ToList();
        var defensePool = availableItems.Where(i => i.itemType == ItemType.Defense).ToList();
        var buffPool = availableItems.Where(i => i.itemType == ItemType.Buff).ToList();

        if (healingPool.Count > 0) 
            finalSelection.Add(healingPool[Random.Range(0, healingPool.Count)]);
        
        if (defensePool.Count > 0) 
            finalSelection.Add(defensePool[Random.Range(0, defensePool.Count)]);
        
        if (buffPool.Count > 0) 
            finalSelection.Add(buffPool[Random.Range(0, buffPool.Count)]);

        List<ShopItemData> remainingItems = availableItems.Except(finalSelection).ToList();

        while (finalSelection.Count < itemsToDisplayCount && remainingItems.Count > 0)
        {
            int randomIndex = Random.Range(0, remainingItems.Count);
            finalSelection.Add(remainingItems[randomIndex]);
            remainingItems.RemoveAt(randomIndex);
        }

        finalSelection = finalSelection.OrderBy(x => Random.value).ToList();

        foreach (var itemData in finalSelection)
        {
            if (itemData == null) continue;

            ShopCard newCard = Instantiate(cardPrefab, itemsContainer);
            newCard.Setup(
                itemData, 
                this, 
                GetBgByType(itemData.itemType), 
                GetColorByType(itemData.itemType)
            );
        }
    }

    private Sprite GetBgByType(ItemType type)
    {
        return type switch
        {
            ItemType.Healing => healingBg,
            ItemType.Defense => defenseBg,
            ItemType.Buff => buffBg,
            _ => null
        };
    }

    private Color GetColorByType(ItemType type)
    {
        return type switch
        {
            ItemType.Healing => healingColor,
            ItemType.Defense => defenseColor,
            ItemType.Buff => buffColor,
            _ => Color.white
        };
    }

    void ShowAlert()
    {
        isAlertActive = true;
        currentAlertTimer = alertTimeWindow;
        shopAlertUI.SetActive(true);
        if (alertAnimator != null) alertAnimator.Rebind();
    }

    void UpdateAlertState()
    {
        currentAlertTimer -= Time.deltaTime;
        if (alertTimerImg != null) alertTimerImg.fillAmount = currentAlertTimer / alertTimeWindow;
        if (currentAlertTimer <= 0) StartCoroutine(CloseAlertRoutine());
    }

    void HandleAlertInput()
    {
        if (Input.GetKeyDown(openKey)) OpenShop();
    }

    IEnumerator CloseAlertRoutine()
    {
        isClosing = true;
        if (alertAnimator != null) alertAnimator.SetTrigger("Close_Shop_Alert");
        yield return new WaitForSeconds(closeAnimDuration);
        shopAlertUI.SetActive(false);
        isAlertActive = false;
        isClosing = false;
        ResetShopTimer();
    }

    public void OpenShop()
    {
        isAlertActive = false;
        shopAlertUI.SetActive(false);
        IsShopOpen = true;
        GenerateShopItems();
        shopUI.SetActive(true);
        Time.timeScale = 0f;
    }

    public void CloseShop()
    {
        IsShopOpen = false;
        shopUI.SetActive(false);
        Time.timeScale = 1f;
        ResetShopTimer();
    }

    void ResetShopTimer()
    {
        nextShopAvailableTime = Time.time + shopCooldown;
    }
}