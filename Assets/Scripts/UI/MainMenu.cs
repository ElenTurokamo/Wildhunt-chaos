using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public MenuTransition transition;

    public void PlayGame()
    {
        transition.Play(); 
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
