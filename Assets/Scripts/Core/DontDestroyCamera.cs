using UnityEngine;

public class DontDestroyCamera : MonoBehaviour
{
    private static DontDestroyCamera instance;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
