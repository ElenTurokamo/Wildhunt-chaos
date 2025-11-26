using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class Bootstrap : MonoBehaviour
{
    public static bool RestartFlag = false;

    private IEnumerator Start()
    {
        yield return SceneManager.LoadSceneAsync("Background", LoadSceneMode.Additive);

        string targetScene;

        if (RestartFlag)
        {
            RestartFlag = false;
            targetScene = "Game";
        }
        else
        {
            targetScene = "Menu";
        }

        PlayerPrefs.SetString("SceneToLoad", targetScene);

        yield return SceneManager.LoadSceneAsync("LoadingScene", LoadSceneMode.Additive);
    }
}