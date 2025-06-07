using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private TMPro.TextMeshProUGUI restartButtonText;
    [SerializeField] private TMPro.TextMeshProUGUI mainMenuButtonText;
    
    void Start()
    {
        // Set cursor state for the menu/game over screen
        Cursor.lockState = CursorLockMode.None; // Unlock the cursor
        Cursor.visible = true; // Make the cursor visible

        Time.timeScale = 1f; 
    }

    // Restarts the game
    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Game");
    }

    // Loads the main menu scene.
    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}