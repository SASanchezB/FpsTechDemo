using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    public Sound[] musicSounds, sfxSounds;
    public AudioSource[] musicSource, sfxSource;

    private AudioClip currentSFXClip;

    [SerializeField] private int musicaIndex;

    private void Awake()
    {

        if (PlayerPrefs.HasKey("ValorSliderMusica"))
        {
            float valorRecuperadoMusica = PlayerPrefs.GetFloat("ValorSliderMusica");
            MusicVolume(valorRecuperadoMusica);
        }

        if (PlayerPrefs.HasKey("ValorSliderSFX"))
        {
            float valorRecuperadoSFX = PlayerPrefs.GetFloat("ValorSliderSFX");
            SFXVolume(valorRecuperadoSFX);
        }

        if (Instance == null)
        {
            Instance = this;


        }

        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        PlayMusic(musicaIndex);
    }



    public void PlayMusic(int index)
    {
        if (musicSource[index] == null)
        {
            Debug.Log("Sound Not Found");
        }

        else
        {
            musicSource[index].Play();
        }

    }

    public void PlaySFX(int index)
    {
        //Sound s = System.Array.Find(sfxSounds, x => x.name == name);

        if (sfxSource[index] == null)
        {
            Debug.Log("Sound Not Found");
        }

        else
        {
            sfxSource[index].Play();
        }

    }

    /*public void ToggleMusic()
    {
        musicSource.mute = !musicSource.mute;

    }

    public void ToggleSFX()
    {
        sfxSource.mute = !sfxSource.mute;
    }*/

    public void MusicVolume(float volume)
    {
        PlayerPrefs.SetFloat("ValorSliderMusica", volume);
        foreach (var item in musicSource)
        {

            item.volume = volume;

        }
    }

    public void SFXVolume(float volume)
    {
        PlayerPrefs.SetFloat("ValorSliderSFX", volume);
        foreach (var item in sfxSource)
        {

            item.volume = volume;

        }
    }

    public void PauseSFX(int index)
    {
        if (sfxSource[index] == null)
        {
            Debug.Log("Sound Not Found");
        }

        else
        {
            sfxSource[index].Pause();
        }
    }

    public void StopSFX(int index)
    {
        if (sfxSource[index] == null)
        {
            Debug.Log("Sound Not Found");
        }

        else
        {
            sfxSource[index].Stop();
            sfxSource[index].loop = false;
        }
    }

    public void PlaySFXLoop(int index) // Nuevo método para reproducir sonido en bucle
    {


        if (sfxSource[index] == null)
        {
            Debug.Log("Sound Not Found");
        }
        else
        {
            sfxSource[index].loop = true;
            sfxSource[index].Play();
        }
    }

    public void PauseSFXLoop(int index)
    {
        if (sfxSource[index] == null)
        {
            Debug.Log("Sound Not Found");
        }
        else
        {
            sfxSource[index].Pause();
            sfxSource[index].Stop();
        }
    }

}