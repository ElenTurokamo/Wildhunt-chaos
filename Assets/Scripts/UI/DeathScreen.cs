using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class DeathScreen : MonoBehaviour
{

    public void Restart()
    {
        Time.timeScale = 1f;
        AudioListener.pause = false;

        Bootstrap.RestartFlag = true;
        SceneManager.LoadScene("Bootstrap");
    }


    private IEnumerator RestartRoutine()
    {
        if (SceneManager.GetSceneByName("Game").isLoaded){
            yield return SceneManager.UnloadSceneAsync("Game");
        }

        if (SceneManager.GetSceneByName("Background").isLoaded){
            yield return SceneManager.UnloadSceneAsync("Background");
        }

        yield return SceneManager.LoadSceneAsync("Background", LoadSceneMode.Additive);
        yield return SceneManager.LoadSceneAsync("Game", LoadSceneMode.Additive);
    }

    public void GoToMenu()
    {
        Time.timeScale = 1f;
        AudioListener.pause = false;
        SceneManager.LoadScene("Bootstrap"); 
    }
}
