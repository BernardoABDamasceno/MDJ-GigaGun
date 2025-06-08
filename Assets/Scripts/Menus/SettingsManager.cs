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
    private const float MAX_VOLUME_LINEAR_VALUE = 10.0f; // Represents +20dB
    private const float MIN_VOLUME_LINEAR_VALUE = 0.0001f; // Represents -80dB (to avoid Log10(0))

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
    }

    void Start()
    {
        // Ensure cursor is visible in settings menu
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Loads saved volume settings and apply them when the scene starts
        LoadVolumeSettings();
    }

    private void OnDestroy()
    {
        // Removes listeners to prevent memory leaks when the GameObject is destroyed
        masterVolumeSlider.onValueChanged.RemoveListener(SetMasterVolume);
        musicVolumeSlider.onValueChanged.RemoveListener(SetMusicVolume);
        sfxVolumeSlider.onValueChanged.RemoveListener(SetSFXVolume);
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
        // Mathf.Log10(volume) * 20f;
        // Here, 'volume' will now range from 0.0001 to 10.0,
        // so Log10 will range from -4 to 1.
        // Multiplying by 20 gives a dB range from -80 to +20.
        float dB = Mathf.Log10(volume) * 20f;
        
        if (mainAudioMixer != null)
        {
            mainAudioMixer.SetFloat("MasterVolume", dB);
            PlayerPrefs.SetFloat("MasterVolume", volume); // Save the linear slider value
        }
    }

    public void SetMusicVolume(float volume)
    {
        float dB = Mathf.Log10(volume) * 20f;
        if (mainAudioMixer != null)
        {
            mainAudioMixer.SetFloat("MusicVolume", dB);
            PlayerPrefs.SetFloat("MusicVolume", volume); // Save the linear slider value
        }
    }

    public void SetSFXVolume(float volume)
    {
        float dB = Mathf.Log10(volume) * 20f;
        if (mainAudioMixer != null)
        {
            mainAudioMixer.SetFloat("SFXVolume", dB);
            PlayerPrefs.SetFloat("SFXVolume", volume); // Save the linear slider value
        }
    }

    // Method to load and apply saved volume settings
    private void LoadVolumeSettings()
    {
        // Master Volume
        // GetFloat has a default value if the key is not found (e.g., first run)
        float masterVol = PlayerPrefs.GetFloat("MasterVolume", 1.0f); // Default to 0dB (linear 1.0)
        masterVolumeSlider.value = masterVol; // Set slider UI
        SetMasterVolume(masterVol); // Apply to mixer

        // Music Volume
        float musicVol = PlayerPrefs.GetFloat("MusicVolume", 1.0f);
        musicVolumeSlider.value = musicVol;
        SetMusicVolume(musicVol);

        // SFX Volume
        float sfxVol = PlayerPrefs.GetFloat("SFXVolume", 1.0f);
        sfxVolumeSlider.value = sfxVol;
        SetSFXVolume(sfxVol);
    }

    // --- Scene Navigation ---
    public void BackToMainMenu()
    {
        PlayButtonClickSound(); // Play sound before transition
        Debug.Log("Going back to main menu from settings...");
        SceneManager.LoadScene("MainMenu"); // Load your MainMenu scene
    }
}