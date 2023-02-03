using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using Cinemachine;

public class SettingsManager : MonoBehaviour
{
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private StoreSetting musicVolume;
    private string musicVolumeKey = "MusicVolume";

    [SerializeField] private Slider sfxVolumeSlider;
    [SerializeField] private StoreSetting sfxVolume;
    private string sfxVolumeKey = "SFXVolume";

    [SerializeField] private AudioMixer mixer;

    public void SetMusicVolume(float percent)
    {
        mixer.SetFloat("MusicVolume", Mathf.Log10(percent) * 20);
        musicVolume.Value = percent;
        PlayerPrefs.SetFloat(musicVolumeKey, percent);
    }

    public void SetSFXVolume(float percent)
    {
        mixer.SetFloat("SFXVolume", Mathf.Log10(percent) * 20);
        sfxVolume.Value = percent;
        PlayerPrefs.SetFloat(sfxVolumeKey, percent);
    }

    private void Start()
    {
        if (!PlayerPrefs.HasKey(musicVolumeKey))
        {
            PlayerPrefs.SetFloat(musicVolumeKey, musicVolume.Value);
            SetMusicVolume(musicVolume.Value);
            musicVolumeSlider.value = musicVolume.Value;
        }
        else
        {
            float musicVol = PlayerPrefs.GetFloat(musicVolumeKey);
            SetMusicVolume(musicVol);
            musicVolumeSlider.value = musicVol;
        }

        if (!PlayerPrefs.HasKey(sfxVolumeKey))
        {
            PlayerPrefs.SetFloat(sfxVolumeKey, sfxVolume.Value);
            SetSFXVolume(sfxVolume.Value);
            sfxVolumeSlider.value = musicVolume.Value;
        }
        else
        {
            float sfxVol = PlayerPrefs.GetFloat(sfxVolumeKey);
            SetSFXVolume(sfxVol);
            sfxVolumeSlider.value = sfxVol;
        }
    }
}
