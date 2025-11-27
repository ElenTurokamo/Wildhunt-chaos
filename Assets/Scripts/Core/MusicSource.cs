using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicSource : MonoBehaviour
{
    [Header("Настройки сцен")]
    [Tooltip("Название звука для сцены 'Menu' (должно быть в AudioManager).")]
    public string menuMusicName = "MenuMusic";
    
    [Tooltip("Название звука для сцены 'Game' (основной геймплей).")]
    public string gameMusicName = "GameMusic";

    [Header("Настройки экрана смерти и босса")]
    [Tooltip("Объект, который сигнализирует о состоянии (например, Panel 'Game Over'). Если он активен, включается Death Music.")]
    public GameObject deathScreenObject;
    
    [Tooltip("Название звука для экрана смерти.")]
    public string deathMusicName = "DeathMusic";
    
    [Space(10)]
    [Tooltip("Объект, который сигнализирует о появлении босса.")]
    public GameObject bossActiveObject;
    
    [Tooltip("Название звука для битвы с боссом.")]
    public string bossMusicName = "BossMusic";


    private string currentPlayingMusic = "";

    void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        CheckAndPlayMusic();
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        CheckAndPlayMusic();
    }
    
    void Update()
    {
        CheckSpecialStateMusic();
    }

    private void CheckAndPlayMusic()
    {
        if (AudioManager.instance == null)
        {
            Debug.LogError("AudioManager.instance не найден!");
            return;
        }

        string sceneName = SceneManager.GetActiveScene().name;
        string targetMusic = "";

        if (sceneName.Contains("Menu")) 
        {
            targetMusic = menuMusicName;
        }
        else if (sceneName.Contains("Game"))
        {
            targetMusic = gameMusicName;
        }
        else
        {
            return; 
        }

        if (targetMusic != currentPlayingMusic)
        {
            StopCurrentMusic();
            AudioManager.instance.Play(targetMusic);
            currentPlayingMusic = targetMusic;
            Debug.Log($"Запущена фоновая музыка: {targetMusic} на сцене: {sceneName}");
        }
    }

    private void CheckSpecialStateMusic()
    {
        if (bossActiveObject != null && bossActiveObject.activeInHierarchy)
        {
            if (currentPlayingMusic != bossMusicName)
            {
                SwitchMusic(bossMusicName);
            }
            return; 
        }
        
        if (deathScreenObject != null && deathScreenObject.activeInHierarchy)
        {
            if (currentPlayingMusic != deathMusicName)
            {
                SwitchMusic(deathMusicName);
            }
        }
        else if (SceneManager.GetActiveScene().name.Contains("Game") && currentPlayingMusic != gameMusicName)
        {
             SwitchMusic(gameMusicName);
        }
    }
    
    private void SwitchMusic(string newMusicName)
    {
        StopCurrentMusic();
        AudioManager.instance.Play(newMusicName);
        currentPlayingMusic = newMusicName;
        Debug.Log($"Музыка переключена на: {newMusicName}");
    }

    private void StopCurrentMusic()
    {
        if (!string.IsNullOrEmpty(currentPlayingMusic))
        {
            AudioManager.instance.Stop(currentPlayingMusic);
        }
    }
}