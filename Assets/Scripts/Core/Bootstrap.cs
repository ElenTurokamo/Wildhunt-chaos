using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class Bootstrap : MonoBehaviour
{
    private IEnumerator Start()
    {
        yield return SceneManager.LoadSceneAsync("Background", LoadSceneMode.Additive);

        yield return SceneManager.LoadSceneAsync("Menu", LoadSceneMode.Additive);

    }
}
