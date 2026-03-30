using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioMixerController : MonoBehaviour
{
    [Header("Audio Mixer")]
    [SerializeField] private AudioMixer audioMixer;

    [Header("UI Sliders")]
    [SerializeField] private Slider sliderMusica;
    [SerializeField] private Slider sliderSFX;

    [Header("Mixer Parameter Names")]
    [SerializeField] private string musicaParameterName = "Musica";
    [SerializeField] private string sfxParameterName = "SFX";

    [Header("PlayerPrefs Keys")]
    private const string MUSIC_VOLUME_KEY = "MusicVolume";
    private const string SFX_VOLUME_KEY = "SFXVolume";

    [Header("Default Values")]
    [SerializeField] private float defaultVolume = 1f; // Volumen maximo (0-1)

    private void Start()
    {
        // Cargar valores guardados
        LoadVolumeSettings();

        // Configurar listeners de los sliders
        if (sliderMusica != null)
        {
            sliderMusica.onValueChanged.AddListener(SetMusicVolume);
        }

        if (sliderSFX != null)
        {
            sliderSFX.onValueChanged.AddListener(SetSFXVolume);
        }
    }

    private void LoadVolumeSettings()
    {
        // Cargar volumen de musica (default 1.0)
        float musicVolume = PlayerPrefs.GetFloat(MUSIC_VOLUME_KEY, defaultVolume);

        // Cargar volumen de SFX (default 1.0)
        float sfxVolume = PlayerPrefs.GetFloat(SFX_VOLUME_KEY, defaultVolume);

        // Aplicar al mixer
        SetMusicVolume(musicVolume);
        SetSFXVolume(sfxVolume);

        // Actualizar sliders si estan asignados
        if (sliderMusica != null)
        {
            sliderMusica.value = musicVolume;
        }

        if (sliderSFX != null)
        {
            sliderSFX.value = sfxVolume;
        }
    }

    public void SetMusicVolume(float volume)
    {
        // Convertir de 0-1 a decibeles (-80 a 0)
        float db = VolumeToDecibels(volume);

        // Aplicar al mixer
        audioMixer.SetFloat(musicaParameterName, db);

        // Guardar en PlayerPrefs
        PlayerPrefs.SetFloat(MUSIC_VOLUME_KEY, volume);
        PlayerPrefs.Save();
    }

    public void SetSFXVolume(float volume)
    {
        // Convertir de 0-1 a decibeles (-80 a 0)
        float db = VolumeToDecibels(volume);

        // Aplicar al mixer
        audioMixer.SetFloat(sfxParameterName, db);

        // Guardar en PlayerPrefs
        PlayerPrefs.SetFloat(SFX_VOLUME_KEY, volume);
        PlayerPrefs.Save();
    }

    // Convertir volumen lineal (0-1) a decibeles (-80 a 0)
    private float VolumeToDecibels(float volume)
    {
        // Clamp entre 0.0001 y 1
        volume = Mathf.Clamp(volume, 0.0001f, 1f);

        // Convertir a decibeles
        return Mathf.Log10(volume) * 20f;
    }

    // Metodos publicos para usar desde codigo
    public float GetMusicVolume()
    {
        return PlayerPrefs.GetFloat(MUSIC_VOLUME_KEY, defaultVolume);
    }

    public float GetSFXVolume()
    {
        return PlayerPrefs.GetFloat(SFX_VOLUME_KEY, defaultVolume);
    }

    public void ResetToDefaults()
    {
        SetMusicVolume(defaultVolume);
        SetSFXVolume(defaultVolume);

        if (sliderMusica != null)
            sliderMusica.value = defaultVolume;

        if (sliderSFX != null)
            sliderSFX.value = defaultVolume;
    }

    // Metodos para mutear completamente
    public void MuteMusic(bool mute)
    {
        if (mute)
        {
            audioMixer.SetFloat(musicaParameterName, -80f);
        }
        else
        {
            float volume = GetMusicVolume();
            SetMusicVolume(volume);
        }
    }

    public void MuteSFX(bool mute)
    {
        if (mute)
        {
            audioMixer.SetFloat(sfxParameterName, -80f);
        }
        else
        {
            float volume = GetSFXVolume();
            SetSFXVolume(volume);
        }
    }

    private void OnDestroy()
    {
        // Limpiar listeners
        if (sliderMusica != null)
        {
            sliderMusica.onValueChanged.RemoveListener(SetMusicVolume);
        }

        if (sliderSFX != null)
        {
            sliderSFX.onValueChanged.RemoveListener(SetSFXVolume);
        }
    }
}