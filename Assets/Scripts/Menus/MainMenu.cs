using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        // Load the "Level1" scene
        SceneManager.LoadScene("Level1");

        InventoryScreen.isPaused = false;
        Time.timeScale = 1.0f;
    }

    public void OpenMainMenu()
    {
        // Load the "MainMenu" scene
        SceneManager.LoadScene("MainMenu");
    }

    public void QuitGame()
    {
        // Logs the message
        Debug.Log("Quit Game");

        // Quit the application (doesnt work in editor but would work in game) 
        Application.Quit();
    }
}

