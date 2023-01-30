using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager _Instance { get; private set; }
    private void Awake()
    {
        if (_Instance != null)
        {
            Destroy(_Instance.gameObject);
        }
        _Instance = this;
    }

    [Header("Music")]
    [SerializeField] private AudioSource musicSource1;
    [SerializeField] private AudioSource musicSource2;
    [SerializeField] private AudioClip[] levelMusic;
    [SerializeField] private float fadeRate = 1f;
    [SerializeField] private float maxMusicVolume = .75f;

    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private TemporaryAudioSource tempSource;

    private AudioClip GetLevelMusic()
    {
        if (GameManager._Instance.OnMainMenu)
        {
            return levelMusic[0];
        }
        else
        {
            return levelMusic[GameManager._Instance.LevelIndex + 1];
        }
    }

    public void StopLevelMusic()
    {
        if (musicSource1.isPlaying)
        {
            StartCoroutine(FadeSource(musicSource1, Direction.DOWN));
        }
        else if (musicSource2.isPlaying)
        {
            StartCoroutine(FadeSource(musicSource2, Direction.DOWN));
        }
    }

    public void StartLevelMusic()
    {
        if (musicSource1.isPlaying)
        {
            musicSource2.clip = GetLevelMusic();
            musicSource2.Play();
            StartCoroutine(FadeSource(musicSource2, Direction.UP));
        }
        else if (musicSource2.isPlaying)
        {
            musicSource1.clip = GetLevelMusic();
            musicSource1.Play();
            StartCoroutine(FadeSource(musicSource1, Direction.UP));
        }
        else
        {
            musicSource1.clip = GetLevelMusic();
            musicSource1.Play();
            StartCoroutine(FadeSource(musicSource1, Direction.UP));
        }
    }

    private IEnumerator FadeSource(AudioSource source, Direction direction)
    {
        if (direction == Direction.UP || direction == Direction.RIGHT)
        {
            while (source.volume < maxMusicVolume)
            {
                source.volume += Time.deltaTime * fadeRate;
                yield return null;
            }
        }
        else
        {
            while (source.volume > 0)
            {
                source.volume -= Time.deltaTime * fadeRate;
                yield return null;
            }
        }
    }

    public void PlayClip(AudioClip clip, float pitch, Vector3 pos)
    {
        Instantiate(tempSource, pos, Quaternion.identity).Play(clip, 1, pitch);
    }

    public void PlayClip(AudioClip clip, bool randomPitch, Vector3 pos)
    {
        PlayClip(clip, randomPitch ? RandomHelper.RandomFloat(0.7f, 1.3f) : 1, pos);
    }

    public void PlayClip(AudioClip clip, bool randomPitch)
    {
        PlayClip(clip, randomPitch ? RandomHelper.RandomFloat(0.7f, 1.3f) : 1);
    }

    public void PlayClip(AudioClip clip, float pitch)
    {
        sfxSource.pitch = pitch;
        sfxSource.PlayOneShot(clip);
    }
}
