using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering;

public class PauseManager : MonoBehaviour
{
    public static bool isGamePaused = false;

    [SerializeField] Canvas pauseCanvas;


    void Start()
    {
        // Ensures the "Game Paused" text is hidden at the start
        if (pauseCanvas != null)
        {
            pauseCanvas.gameObject.SetActive(false);
        }

        Time.timeScale = 1f;
        isGamePaused = false;

        // Lock and hide the cursor at the start of the game
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // Check for the "Tab" key press
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (isGamePaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void PauseGame()
    {
        if (pauseCanvas != null)
        {
            pauseCanvas.gameObject.SetActive(true);
        }

        Time.timeScale = 0f; // Stop time
        isGamePaused = true;

        // Unlock and show the cursor when paused
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ResumeGame()
    {
        if (pauseCanvas != null)
        {
            pauseCanvas.gameObject.SetActive(false);
        }

        Time.timeScale = 1f;
        isGamePaused = false;

        // Re-lock and hide the cursor for gameplay
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Loads the main menu scene.
    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}