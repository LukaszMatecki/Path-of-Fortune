using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeSettings : MonoBehaviour
{
    [SerializeField] private AudioMixer myMixer;
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider musicSlider2;
    [SerializeField] private Slider SFXSlider;

    private void Start()
    {
        if (PlayerPrefs.HasKey("musicVolume"))
        {
            LoadVolume();
        }
        else
        {
            masterSlider.value = 0.5f;
            musicSlider.value = 0.5f;
            musicSlider2.value = 0.5f;
            SFXSlider.value = 0.5f;

            SetMasterVolume();
            SetMusicVolume();
            SetSFXVolume();
        }

        // Dodaj nas³uchiwanie zmian wartoœci dla synchronizacji
        musicSlider.onValueChanged.AddListener(SynchronizeMusicSliders);
        musicSlider2.onValueChanged.AddListener(SynchronizeMusicSliders);
    }

    public void SetMasterVolume()
    {
        float volume = masterSlider.value;
        myMixer.SetFloat("master", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("masterVolume", volume);
    }

    public void SetMusicVolume()
    {
        float volume = musicSlider.value;
        myMixer.SetFloat("music", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("musicVolume", volume);
    }

    public void SetSFXVolume()
    {
        float volume = SFXSlider.value;
        myMixer.SetFloat("SFX", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("SFXVolume", volume);
    }

    private void SynchronizeMusicSliders(float value)
    {
        // Ustaw tê sam¹ wartoœæ w obu sliderach
        if (musicSlider.value != value) musicSlider.value = value;
        if (musicSlider2.value != value) musicSlider2.value = value;

        // Aktualizuj g³oœnoœæ
        SetMusicVolume();
    }

    private void LoadVolume()
    {
        masterSlider.value = PlayerPrefs.GetFloat("masterVolume");
        musicSlider.value = PlayerPrefs.GetFloat("musicVolume");
        musicSlider2.value = PlayerPrefs.GetFloat("musicVolume");
        SFXSlider.value = PlayerPrefs.GetFloat("SFXVolume");

        SetMasterVolume();
        SetMusicVolume();
        SetSFXVolume();
    }
}