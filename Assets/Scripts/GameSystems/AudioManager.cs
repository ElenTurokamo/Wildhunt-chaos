using UnityEngine;
using UnityEngine.Audio;
using System;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    public AudioMixer mainMixer;
    public Sound[] sounds;

    // Ключи для сохранения настроек
    private const string MASTER_KEY = "MasterVolume";
    private const string MUSIC_KEY = "MusicVolume";
    private const string SFX_KEY = "EffectsVolume";

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

    public void Play(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }
        s.source.Play();
    }

    public void Stop(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s != null) s.source.Stop();
    }

    // Методы для UI слайдеров (значения от -80 до 0 или -80 до 20)
    public void SetMasterVolume(float volume)
    {
        mainMixer.SetFloat(MASTER_KEY, volume);
        PlayerPrefs.SetFloat(MASTER_KEY, volume);
    }

    public void SetMusicVolume(float volume)
    {
        mainMixer.SetFloat(MUSIC_KEY, volume);
        PlayerPrefs.SetFloat(MUSIC_KEY, volume);
    }

    public void SetEffectsVolume(float volume)
    {
        mainMixer.SetFloat(SFX_KEY, volume);
        PlayerPrefs.SetFloat(SFX_KEY, volume);
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
}