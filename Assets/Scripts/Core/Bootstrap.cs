using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class Bootstrap : MonoBehaviour
{
    public static bool RestartFlag = false;

    private const string LoadingSceneName = "LoadingScene"; 
    private const string GameSceneName = "Game";
    private const string MenuSceneName = "Menu";

    private IEnumerator Start()
    {
        yield return SceneManager.LoadSceneAsync("Background", LoadSceneMode.Additive);

        string targetScene;
        if (RestartFlag)
        {
            RestartFlag = false;
            targetScene = GameSceneName;
        }
        else
        {
            targetScene = MenuSceneName;
        }

        PlayerPrefs.SetString("SceneToLoad", targetScene); 
        
        SceneManager.LoadScene(LoadingSceneName); 
    }
}