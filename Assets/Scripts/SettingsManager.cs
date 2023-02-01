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

    [SerializeField] private Slider sfxVolumeSlider;
    [SerializeField] private StoreSetting sfxVolume;

    [SerializeField] private AudioMixer mixer;

    public void SetMusicVolume(float percent)
    {
        mixer.SetFloat("MusicVolume", Mathf.Log10(percent) * 20);
        musicVolume.Value = percent;
    }

    public void SetSFXVolume(float percent)
    {
        mixer.SetFloat("SFXVolume", Mathf.Log10(percent) * 20);
        sfxVolume.Value = percent;
    }

    private void Start()
    {
        SetMusicVolume(musicVolume.Value);
        musicVolumeSlider.value = musicVolume.Value;

        SetSFXVolume(sfxVolume.Value);
        sfxVolumeSlider.value = sfxVolume.Value;
    }
}
