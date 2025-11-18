using UnityEngine;
using UnityEngine.Rendering.Universal;
using System.Collections;

public class LightFader : MonoBehaviour
{
    public Light2D globalLight;

    public Color menuColor = new Color(1f, 1f, 1f);
    public Color gameColor = new Color(0.7f, 0.9f, 1f);

    public float fadeDuration = 3f;

    private Coroutine fadeCoroutine;

    public void FadeToGameColor()
    {
        StartFade(gameColor);
    }

    public void FadeToMenuColor()
    {
        StartFade(menuColor);
    }

    private void StartFade(Color targetColor)
    {
        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        fadeCoroutine = StartCoroutine(FadeCoroutine(targetColor));
    }

    private IEnumerator FadeCoroutine(Color targetColor)
    {
        Color startColor = globalLight.color;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime / fadeDuration;
            globalLight.color = Color.Lerp(startColor, targetColor, t);
            yield return null;
        }

        globalLight.color = targetColor; // точно устанавливаем финальный цвет
    }
}
