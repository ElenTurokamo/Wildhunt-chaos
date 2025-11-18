using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

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

        // Запуск изменения цвета света
        lightFader.FadeToGameColor();

        while (t < 1f)
        {
            t += Time.deltaTime / duration;

            uiRoot.localPosition = Vector3.Lerp(startPos, targetPos, t);
            ui.alpha = 1 - t;

            yield return null;
        }

        // Загружаем GameScene поверх BackgroundScene
        yield return SceneManager.LoadSceneAsync("Game", LoadSceneMode.Additive);

        // Выгружаем MenuScene
        SceneManager.UnloadSceneAsync("Menu");
    }
}
