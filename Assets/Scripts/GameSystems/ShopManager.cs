using UnityEngine;
using UnityEngine.UI; 
using System.Collections;

public class ShopManager : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject shopAlertUI;
    public Image alertTimerImg; 
    public GameObject shopUI;
    public Animator alertAnimator;

    [Header("References")]
    public WaveController waveController;
    
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
                float timeToElite = waveController.GetTimeUntilElite();
                if (timeToElite <= alertTimeWindow && timeToElite > 0)
                {
                    ShowAlert();
                }
            }
        }
        else if (isAlertActive && !isClosing)
        {
            UpdateAlertState();
            HandleAlertInput();
        }
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
        
        if (alertTimerImg != null)
        {
            alertTimerImg.fillAmount = currentAlertTimer / alertTimeWindow;
        }

        if (currentAlertTimer <= 0)
        {
            StartCoroutine(CloseAlertRoutine());
        }
    }

    void HandleAlertInput()
    {
        if (Input.GetKeyDown(openKey))
        {
            OpenShop();
        }
    }

    IEnumerator CloseAlertRoutine()
    {
        isClosing = true;
        
        if (alertAnimator != null)
        {
            alertAnimator.SetTrigger("Close_Shop_Alert");
        }

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