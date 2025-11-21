using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.EventSystems;

public class MenuTransition : MonoBehaviour
{
    public CanvasGroup ui;
    public Transform uiRoot;   
    public float moveDistance = 1000f;
    public float duration = 1f;

    void Start()
    {
        
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

        while (t < 1f)
        {
            t += Time.deltaTime / duration;

            uiRoot.localPosition = Vector3.Lerp(startPos, targetPos, t);
            ui.alpha = 1 - t;

            yield return null;
        }

        var eventSystem = FindFirstObjectByType<EventSystem>();
        if (eventSystem != null)
            Destroy(eventSystem.gameObject);

        yield return SceneManager.LoadSceneAsync("Game", LoadSceneMode.Additive);

        SceneManager.UnloadSceneAsync("Menu");
    }
}