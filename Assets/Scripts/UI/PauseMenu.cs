using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseUI;
    private bool isPaused = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused) Resume();
            else Pause();
        }
    }

    public void Resume()
    {
        pauseUI.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
        
        if (AudioManager.instance != null)
            AudioManager.instance.SetMuffled(false);
    }

    void Pause()
    {
        pauseUI.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;

        if (AudioManager.instance != null)
            AudioManager.instance.SetMuffled(true);
    }

    public void GoToMenu()
    {
        Time.timeScale = 1f;
        AudioListener.pause = false;

        if (AudioManager.instance != null)
            AudioManager.instance.SetMuffled(false, 0.1f);

        SceneManager.LoadScene("Bootstrap", LoadSceneMode.Single);
    }
}