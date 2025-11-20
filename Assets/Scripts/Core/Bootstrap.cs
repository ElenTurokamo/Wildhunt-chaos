using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class Bootstrap : MonoBehaviour
{
    public static bool RestartFlag = false;

    private IEnumerator Start()
    {
        yield return SceneManager.LoadSceneAsync("Background", LoadSceneMode.Additive);

        if (RestartFlag)
        {
            RestartFlag = false;

            yield return SceneManager.LoadSceneAsync("Game", LoadSceneMode.Additive);
        }
        else
        {
            yield return SceneManager.LoadSceneAsync("Menu", LoadSceneMode.Additive);
        }
    }
}