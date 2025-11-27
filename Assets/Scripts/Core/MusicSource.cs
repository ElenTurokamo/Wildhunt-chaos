using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicSource : MonoBehaviour
{
    [Header("Названия треков")]
    public string menuMusicName = "MenuMusic";
    public string gameMusicName = "GameMusic";
    public string deathMusicName = "DeathMusic";
    public string bossMusicName = "BossMusicBoss";

    [Header("Настройки")]
    public float musicFadeTime = 1.5f; 

    [Header("Автопоиск")]
    public string deathScreenTag = "DeathScreen";
    public string bossObjectTag = "BossActive";

    private GameObject deathScreenObject;
    private GameObject bossActiveObject;    

    private string currentTargetMusic = "NONE"; // Изначально ставим "NONE", чтобы при старте сработала логика
    private string sceneBackgroundMusic = ""; 

    void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        CheckSceneMusic(SceneManager.GetActiveScene().name);
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        AutoFindObjects();
        CheckSceneMusic(scene.name);
    }

    void Update()
    {
        CheckSpecialStates();
    }

    private void AutoFindObjects()
    {
        deathScreenObject = GameObject.FindGameObjectWithTag(deathScreenTag);
        bossActiveObject = GameObject.FindGameObjectWithTag(bossObjectTag);
    }

    private void CheckSceneMusic(string sceneName)
    {
        // 1. Если это Меню
        if (sceneName.Contains("Menu"))
        {
            sceneBackgroundMusic = menuMusicName;
        }
        // 2. Если это Игра
        else if (sceneName.Contains("Game"))
        {
            sceneBackgroundMusic = gameMusicName;
        }
        // 3. Если это ЗАГРУЗКА -> Хотим тишину
        else if (sceneName.Contains("LoadingScene"))
        {
            sceneBackgroundMusic = ""; // Пустая строка будет означать "Стоп"
        }
        // Для всех остальных сцен (Background, Bootstrap) ничего не меняем, 
        // оставляем музыку, которая была назначена последней значимой сценой.
    }

    private void CheckSpecialStates()
    {
        string desiredMusic = sceneBackgroundMusic; 

        // Приоритеты (Смерть и Босс)
        if (deathScreenObject != null && deathScreenObject.activeInHierarchy)
        {
            desiredMusic = deathMusicName;
        }
        else if (bossActiveObject != null && bossActiveObject.activeInHierarchy)
        {
            desiredMusic = bossMusicName;
        }

        // Логика переключения
        if (desiredMusic != currentTargetMusic)
        {
            SwitchTo(desiredMusic);
        }
    }

    private void SwitchTo(string musicName)
    {
        currentTargetMusic = musicName;

        if (AudioManager.instance == null) return;

        // Если имя музыки пустое — значит, нужно остановить музыку (Fade Out)
        if (string.IsNullOrEmpty(musicName))
        {
            AudioManager.instance.StopMusic(musicFadeTime);
        }
        else
        {
            // Иначе включаем новую (Crossfade)
            AudioManager.instance.PlayMusic(musicName, musicFadeTime);
        }
    }
}