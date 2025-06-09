using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public Slider _musicSlider, _sfxSlider, _xSen, _ySen;
    private PlayerCam playerCam;

    private void Awake()
    {
        // Buscamos el componente PlayerCam en la escena
        playerCam = FindObjectOfType<PlayerCam>();

        if (PlayerPrefs.HasKey("ValorSliderMusica"))
            _musicSlider.value = PlayerPrefs.GetFloat("ValorSliderMusica");

        if (PlayerPrefs.HasKey("ValorSliderSFX"))
            _sfxSlider.value = PlayerPrefs.GetFloat("ValorSliderSFX");

        if (PlayerPrefs.HasKey("ValorSliderxSen"))
            _xSen.value = PlayerPrefs.GetFloat("ValorSliderxSen");

        if (PlayerPrefs.HasKey("ValorSliderySen"))
            _ySen.value = PlayerPrefs.GetFloat("ValorSliderySen");

        // Aplicamos las sensibilidades al PlayerCam
        if (playerCam != null)
        {
            playerCam.sensX = _xSen.value;
            playerCam.sensY = _ySen.value;
        }
    }

    public void MusicVolume()
    {
        AudioManager.Instance.MusicVolume(_musicSlider.value);
    }

    public void SFXVolume()
    {
        AudioManager.Instance.SFXVolume(_sfxSlider.value);
    }

    public void xSen()
    {
        float value = _xSen.value;
        PlayerPrefs.SetFloat("ValorSliderxSen", value);
        if (playerCam != null)
            playerCam.sensX = value;
    }

    public void ySen()
    {
        float value = _ySen.value;
        PlayerPrefs.SetFloat("ValorSliderySen", value);
        if (playerCam != null)
            playerCam.sensY = value;
    }
}
