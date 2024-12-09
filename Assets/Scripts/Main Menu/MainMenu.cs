using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenuUI; // Panel Pause Menu
    public string lobbyMenuSceneName = "Lobby Menu"; // Scene name for Lobby Menu

    private bool isPaused = false;

    void Update()
    {
        // Deteksi jika ESC ditekan
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    private void TogglePause()
    {
        if (isPaused)
        {
            Resume();
        }
        else
        {
            Pause();
        }
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f; // Kembalikan waktu normal
        isPaused = false;
    }

    public void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f; // Hentikan waktu
        isPaused = true;
    }

    public void QuitGame()
    {
        Time.timeScale = 1f; // Reset time scale before loading new scene
        SceneManager.LoadScene(lobbyMenuSceneName);
    }
}