using UnityEngine;

public class QuitGame : MonoBehaviour
{
    void Update()
    {
        // Check if the Backspace key was pressed down in this frame
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            DoQuitGame();
        }
    }

    public void DoQuitGame()
    {
#if UNITY_EDITOR
        // If running in the Unity editor, stop playing
        UnityEditor.EditorApplication.isPlaying = false;
#else
        // If running as a standalone build, quit the application
        Application.Quit();
#endif
    }
}