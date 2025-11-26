using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LoadingManager : MonoBehaviour
{
    private void Start()
    {
        string sceneToLoad = PlayerPrefs.GetString("SceneToLoad", "Menu"); 
        StartCoroutine(LoadAsyncScene(sceneToLoad));
    }

    private IEnumerator LoadAsyncScene(string sceneName)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive); 
        operation.allowSceneActivation = false; 

        while (!operation.isDone)
        {
            if (operation.progress >= 0.9f)
            {
                yield return new WaitForSeconds(1.5f); 
                
                operation.allowSceneActivation = true;
            }

            yield return null;
        }

        SceneManager.UnloadSceneAsync(gameObject.scene);
    }
}