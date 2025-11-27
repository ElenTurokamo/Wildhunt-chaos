using UnityEngine;
using UnityEngine.Audio;
using System;
using System.Collections;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    public AudioMixer mainMixer;
    public Sound[] sounds;

    private const string MASTER_KEY = "MasterVolume";
    private const string MUSIC_KEY = "MusicVolume";
    private const string SFX_KEY = "EffectsVolume";

    private AudioSource currentMusicSource; 
    private Coroutine fadeCoroutine; 

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.outputAudioMixerGroup = s.mixerGroup;
            s.source.volume = s.volume; 
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }
    }

    void Start()
    {
        LoadVolume();
    }

    public void PlaySFX(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null) return;
        s.source.Play();
    }

    public void PlayMusic(string name, float fadeDuration = 1.0f)
    {
        Sound nextSound = Array.Find(sounds, sound => sound.name == name);
        if (nextSound == null) return;

        if (currentMusicSource == nextSound.source && currentMusicSource.isPlaying)
            return;

        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        fadeCoroutine = StartCoroutine(CrossFadeMusic(nextSound, fadeDuration));
    }

    // --- НОВЫЙ МЕТОД: Остановка музыки с Fade Out ---
    public void StopMusic(float fadeDuration = 1.0f)
    {
        if (currentMusicSource == null || !currentMusicSource.isPlaying) return;

        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        fadeCoroutine = StartCoroutine(FadeOutAndStop(fadeDuration));
    }

    private IEnumerator FadeOutAndStop(float duration)
    {
        float startVolume = currentMusicSource.volume;
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            // Плавно уменьшаем громкость до 0
            currentMusicSource.volume = Mathf.Lerp(startVolume, 0f, timer / duration);
            yield return null;
        }

        currentMusicSource.Stop();
        currentMusicSource.volume = startVolume; // Возвращаем громкость для будущего использования
        currentMusicSource = null;
    }
    // ------------------------------------------------

    private IEnumerator CrossFadeMusic(Sound nextSound, float duration)
    {
        float timer = 0f;
        AudioSource nextSource = nextSound.source;
        
        nextSource.volume = 0f; 
        nextSource.Play();

        AudioSource oldSource = currentMusicSource;
        float targetVolume = nextSound.volume; 
        float startOldVolume = (oldSource != null) ? oldSource.volume : 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            float percent = timer / duration;

            nextSource.volume = Mathf.Lerp(0f, targetVolume, percent);

            if (oldSource != null)
            {
                oldSource.volume = Mathf.Lerp(startOldVolume, 0f, percent);
            }

            yield return null;
        }

        if (oldSource != null)
        {
            oldSource.Stop();
            oldSource.volume = startOldVolume;
        }

        nextSource.volume = targetVolume;
        currentMusicSource = nextSource;
    }

    public void SetMasterVolume(float volume)
    {
        mainMixer.SetFloat(MASTER_KEY, volume);
        PlayerPrefs.SetFloat(MASTER_KEY, volume);
        PlayerPrefs.Save();
    }

    public void SetMusicVolume(float volume)
    {
        mainMixer.SetFloat(MUSIC_KEY, volume);
        PlayerPrefs.SetFloat(MUSIC_KEY, volume);
        PlayerPrefs.Save();
    }

    public void SetEffectsVolume(float volume)
    {
        mainMixer.SetFloat(SFX_KEY, volume);
        PlayerPrefs.SetFloat(SFX_KEY, volume);
        PlayerPrefs.Save();
    }

    private void LoadVolume()
    {
        float masterVol = PlayerPrefs.GetFloat(MASTER_KEY, 0f);
        float musicVol = PlayerPrefs.GetFloat(MUSIC_KEY, 0f);
        float sfxVol = PlayerPrefs.GetFloat(SFX_KEY, 0f);

        mainMixer.SetFloat(MASTER_KEY, masterVol);
        mainMixer.SetFloat(MUSIC_KEY, musicVol);
        mainMixer.SetFloat(SFX_KEY, sfxVol);
    }

    public void SaveSettings()
    {
        PlayerPrefs.Save();
    }
}


