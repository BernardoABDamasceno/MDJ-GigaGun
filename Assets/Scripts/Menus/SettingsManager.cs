using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class SettingsManager : MonoBehaviour
{
    [Header("Audio Mixer")]
    [SerializeField] private AudioMixer mainAudioMixer;

    [Header("Volume Sliders")]
    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;

    [Header("Audio")]
    [SerializeField] private AudioClip buttonClickSound;
    private AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.loop = false;
        }
    }

    void Start()
    {
        
        // Ensure cursor is visible in settings menu
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void PlayButtonClickSound()
    {
        if (audioSource != null && buttonClickSound != null)
        {
            audioSource.PlayOneShot(buttonClickSound);
        }
        else
        {
            if (audioSource == null) Debug.LogWarning("AudioSource not found on SettingsManager for button click sound.");
            if (buttonClickSound == null) Debug.LogWarning("Button Click Sound AudioClip not assigned in SettingsManager.");
        }
    }

    // --- Volume Control Methods ---
    public void SetMasterVolume(float volume)
    {
        if (mainAudioMixer != null)
        {
            mainAudioMixer.SetFloat("MasterVolume", Mathf.Log10(Mathf.Max(volume, 0.0001f)) * 20f);
            PlayerPrefs.SetFloat("MasterVolume", volume); // Save setting
        }
    }

    public void SetMusicVolume(float volume)
    {
        if (mainAudioMixer != null)
        {
            mainAudioMixer.SetFloat("MusicVolume", Mathf.Log10(Mathf.Max(volume, 0.0001f)) * 20f);
            PlayerPrefs.SetFloat("MusicVolume", volume); // Save setting
        }
    }

    public void SetSFXVolume(float volume)
    {
        if (mainAudioMixer != null)
        {
            mainAudioMixer.SetFloat("SFXVolume", Mathf.Log10(Mathf.Max(volume, 0.0001f)) * 20f);
            PlayerPrefs.SetFloat("SFXVolume", volume); // Save setting
        }
    }

    // --- Scene Navigation ---
    public void BackToMainMenu()
    {
        PlayButtonClickSound(); // Play sound before transition
        Debug.Log("Going back to main menu from settings...");
        SceneManager.LoadScene("MainMenu"); // Load your MainMenu scene
    }

}