using UnityEngine;

public class DontDestroyCamera : MonoBehaviour
{
    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
}
