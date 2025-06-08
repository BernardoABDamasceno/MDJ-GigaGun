using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    public static bool isGamePaused = false;

    [SerializeField] private GameObject pauseTextUI;
    [SerializeField] private TMPro.TextMeshProUGUI mainMenuButtonText;


    void Start()
    {
        // Ensures the "Game Paused" text is hidden at the start
        if (pauseTextUI != null)
        {
            pauseTextUI.SetActive(false);
        }

        if (mainMenuButtonText != null)
        {
            mainMenuButtonText.gameObject.SetActive(false);
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
        if (pauseTextUI != null)
        {
            pauseTextUI.SetActive(true);
        }

        if (mainMenuButtonText != null)
        {
            mainMenuButtonText.gameObject.SetActive(true);
        }

        Time.timeScale = 0f; // Stop time
        isGamePaused = true;

        // Unlock and show the cursor when paused
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ResumeGame()
    {
        if (pauseTextUI != null)
        {
            pauseTextUI.SetActive(false);
        }

        if (mainMenuButtonText != null)
        {
            mainMenuButtonText.gameObject.SetActive(false);
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