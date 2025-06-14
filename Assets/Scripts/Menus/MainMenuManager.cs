using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using TMPro;

public class MainMenuManager : MonoBehaviour
{
    void Start()
    {
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.None; // Make the cursor visible and unlocked
        Cursor.visible = true;

    }

    /// Loads the main game scene.
    public void StartGame()
    {
        Debug.Log("Starting game...");
        SceneManager.LoadScene("Mojo");
    }
    /// Called when the "SETTINGS" button is pressed.
    public void OpenSettings()
    {
        Debug.Log("Opening settings menu...");
        SceneManager.LoadScene("Settings");
    }
    /// Called when the "Credits" button is pressed.
    public void OpenCredits()
    {
        Debug.Log("Opening credits menu...");
        SceneManager.LoadScene("Credits");
    }
    /// Called when the "CLOSE GAME" button is pressed.
    public void CloseGame() // Renamed from QuitGame
    {
        Debug.Log("Closing game...");
        Application.Quit();

        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // Stops play mode in editor
        #endif
    }
}