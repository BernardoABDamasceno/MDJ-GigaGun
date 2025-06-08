using System.Collections;
using System.Collections.Generic;
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

    // Constants for volume ranges
    private const float MAX_VOLUME_LINEAR_VALUE = 2.0f;
    private const float MIN_VOLUME_LINEAR_VALUE = 0.0001f; // Represents -80dB (to avoid Log10(0))

    [Header("Display Settings")]
    [SerializeField] private Button fullscreenButton;
    [SerializeField] private Button windowedButton;

    [Tooltip("The preferred width for windowed mode.")]
    [SerializeField] private int windowedWidth = 1280; // Default width for windowed mode
    [Tooltip("The preferred height for windowed mode.")]
    [SerializeField] private int windowedHeight = 720; // Default height for windowed mode

    [Tooltip("The preferred width for fullscreen mode.")]
    [SerializeField] private int fullscreenWidth = 1920; // Default width for fullscreen mode
    [Tooltip("The preferred height for fullscreen mode.")]
    [SerializeField] private int fullscreenHeight = 1080; // Default height for fullscreen mode

    // CRT Filter Settings
    [Header("CRT Filter")]
    [SerializeField] private Button crtOnButton;
    [SerializeField] private Button crtOffButton;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.loop = false;
        }
        // Sets slider ranges programmatically for consistency
        masterVolumeSlider.minValue = MIN_VOLUME_LINEAR_VALUE;
        masterVolumeSlider.maxValue = MAX_VOLUME_LINEAR_VALUE;
        musicVolumeSlider.minValue = MIN_VOLUME_LINEAR_VALUE;
        musicVolumeSlider.maxValue = MAX_VOLUME_LINEAR_VALUE;
        sfxVolumeSlider.minValue = MIN_VOLUME_LINEAR_VALUE;
        sfxVolumeSlider.maxValue = MAX_VOLUME_LINEAR_VALUE;
        // Adds listeners for slider value changes.
        masterVolumeSlider.onValueChanged.AddListener(SetMasterVolume);
        musicVolumeSlider.onValueChanged.AddListener(SetMusicVolume);
        sfxVolumeSlider.onValueChanged.AddListener(SetSFXVolume);

        fullscreenButton.onClick.AddListener(SetFullscreen);
        windowedButton.onClick.AddListener(SetWindowed);

        // CRT Button Listener
        crtOnButton.onClick.AddListener(CrtOn);
        crtOffButton.onClick.AddListener(CrtOff);
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        LoadVolumeSettings();
        LoadDisplaySettings();

    }

    private void OnDestroy()
    {
        masterVolumeSlider.onValueChanged.RemoveListener(SetMasterVolume);
        musicVolumeSlider.onValueChanged.RemoveListener(SetMusicVolume);
        sfxVolumeSlider.onValueChanged.RemoveListener(SetSFXVolume);

        fullscreenButton.onClick.RemoveListener(SetFullscreen);
        windowedButton.onClick.RemoveListener(SetWindowed);

        // CRT Button Listeners
        crtOnButton.onClick.RemoveListener(CrtOn);
        crtOffButton.onClick.RemoveListener(CrtOff);
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
        float dB = Mathf.Log10(volume) * 20f;
        
        if (mainAudioMixer != null)
        {
            mainAudioMixer.SetFloat("MasterVolume", dB);
            PlayerPrefs.SetFloat("MasterVolume", volume);
        }
    }

    public void SetMusicVolume(float volume)
    {
        float dB = Mathf.Log10(volume) * 20f;
        if (mainAudioMixer != null)
        {
            mainAudioMixer.SetFloat("MusicVolume", dB);
            PlayerPrefs.SetFloat("MusicVolume", volume);
        }
    }

    public void SetSFXVolume(float volume)
    {
        float dB = Mathf.Log10(volume) * 20f;
        if (mainAudioMixer != null)
        {
            mainAudioMixer.SetFloat("SFXVolume", dB);
            PlayerPrefs.SetFloat("SFXVolume", volume);
        }
    }

    private void LoadVolumeSettings()
    {
        float masterVol = PlayerPrefs.GetFloat("MasterVolume", 1.0f);
        masterVolumeSlider.value = masterVol;
        SetMasterVolume(masterVol);

        float musicVol = PlayerPrefs.GetFloat("MusicVolume", 1.0f);
        musicVolumeSlider.value = musicVol;
        SetMusicVolume(musicVol);

        float sfxVol = PlayerPrefs.GetFloat("SFXVolume", 1.0f);
        sfxVolumeSlider.value = sfxVol;
        SetSFXVolume(sfxVol);
    }

    // Display Control Methods
    public void SetFullscreen()
    {
        PlayButtonClickSound();
        Screen.SetResolution(fullscreenWidth, fullscreenHeight, FullScreenMode.FullScreenWindow);
        Screen.fullScreen = true;

        PlayerPrefs.SetInt("Fullscreen", 1);
        PlayerPrefs.SetInt("FullscreenWidth", fullscreenWidth);
        PlayerPrefs.SetInt("FullscreenHeight", fullscreenHeight);
        Debug.Log($"Set to Fullscreen: {fullscreenWidth}x{fullscreenHeight}");
    }

    public void SetWindowed()
    {
        PlayButtonClickSound();
        Screen.SetResolution(windowedWidth, windowedHeight, FullScreenMode.Windowed);
        Screen.fullScreen = false;

        PlayerPrefs.SetInt("Fullscreen", 0);
        PlayerPrefs.SetInt("WindowedWidth", windowedWidth);
        PlayerPrefs.SetInt("WindowedHeight", windowedHeight);
        Debug.Log($"Set to Windowed: {windowedWidth}x{windowedHeight}");
    }

    private void LoadDisplaySettings()
    {
        windowedWidth = PlayerPrefs.GetInt("WindowedWidth", windowedWidth);
        windowedHeight = PlayerPrefs.GetInt("WindowedHeight", windowedHeight);

        fullscreenWidth = PlayerPrefs.GetInt("FullscreenWidth", fullscreenWidth);
        fullscreenHeight = PlayerPrefs.GetInt("FullscreenHeight", fullscreenHeight);

        int isFullscreen = PlayerPrefs.GetInt("Fullscreen", 1); // Default to fullscreen

        if (isFullscreen == 1)
        {
            Screen.SetResolution(fullscreenWidth, fullscreenHeight, FullScreenMode.FullScreenWindow);
            Screen.fullScreen = true;
        }
        else
        {
            Screen.SetResolution(windowedWidth, windowedHeight, FullScreenMode.Windowed);
            Screen.fullScreen = false;
        }
        Debug.Log($"Loaded Display Setting: Fullscreen = {Screen.fullScreen}, Resolution = {Screen.width}x{Screen.height}");
    }

    // CRT Filter Control Methods
    public void CrtOn()
    {
        PlayButtonClickSound();
        PlayerPrefs.SetInt("CrtOn", 1); // Save CRT state (1 = on, 0 = off)
        PlayerPrefs.Save(); // Ensures PlayerPrefs are written to disk immediately
        Debug.Log("CRT Filter On preference saved.");
    }

    public void CrtOff()
    {
        PlayButtonClickSound();
        PlayerPrefs.SetInt("CrtOn", 0); // Save CRT state (1 = on, 0 = off)
        PlayerPrefs.Save(); // Ensures PlayerPrefs are written to disk immediately
        Debug.Log("CRT Filter Off preference saved.");
    }
    public void BackToMainMenu()
    {
        PlayButtonClickSound();
        Debug.Log("Going back to main menu from settings...");
        SceneManager.LoadScene("MainMenu");
    }
}