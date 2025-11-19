using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.EventSystems;

public class MenuTransition : MonoBehaviour
{
    public CanvasGroup ui;
    public Transform uiRoot;    // объект, который будет улетать вверх
    public float moveDistance = 1000f;
    public float duration = 1f;

    public LightFader lightFader; // (создадим ниже)

    void Start()
    {
        lightFader = FindFirstObjectByType<LightFader>();
    }

    public void Play()
    {
        StartCoroutine(Transition());
    }

    IEnumerator Transition()
    {
        float t = 0f;
        Vector3 startPos = uiRoot.localPosition;
        Vector3 targetPos = startPos + new Vector3(0, moveDistance, 0);

        lightFader.FadeToGameColor();

        while (t < 1f)
        {
            t += Time.deltaTime / duration;

            uiRoot.localPosition = Vector3.Lerp(startPos, targetPos, t);
            ui.alpha = 1 - t;

            yield return null;
        }

        // УНИЧТОЖАЕМ EventSystem из Menu СРАЗУ
        var eventSystem = FindFirstObjectByType<EventSystem>();
        if (eventSystem != null)
            Destroy(eventSystem.gameObject);

        // Теперь грузим Game — ЧИСТО, СПОКОЙНО
        yield return SceneManager.LoadSceneAsync("Game", LoadSceneMode.Additive);

        // И только потом — Unload Menu
        SceneManager.UnloadSceneAsync("Menu");
    }
}