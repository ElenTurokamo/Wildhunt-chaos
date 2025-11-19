using UnityEngine;
using System.Collections;

public class AttachMainCamera : MonoBehaviour
{
    void Start()
    {
        var canvas = GetComponent<Canvas>();

        // Ждём 1 кадр, чтобы камера успела появиться
        StartCoroutine(Assign());
    }

    IEnumerator Assign()
    {
        yield return null;

        var canvas = GetComponent<Canvas>();
        if (canvas != null)
        {
            canvas.worldCamera = Camera.main;
        }
    }
}
