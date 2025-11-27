using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    public Slider masterSlider;
    public Slider musicSlider;
    public Slider sfxSlider;

    private const string MASTER_KEY = "MasterVolume";
    private const string MUSIC_KEY = "MusicVolume";
    private const string SFX_KEY = "EffectsVolume";

    void Start()
    {
        masterSlider.value = PlayerPrefs.GetFloat(MASTER_KEY, 0f);
        musicSlider.value = PlayerPrefs.GetFloat(MUSIC_KEY, 0f);
        sfxSlider.value = PlayerPrefs.GetFloat(SFX_KEY, 0f);
    }
}