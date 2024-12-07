using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenuUI; // Panel Pause Menu
    public GameObject mapsPanelUI; // Panel Maps

    private bool isPaused = false;

    void Update()
    {
        // Deteksi jika ESC ditekan
        if (Input.GetKeyDown(KeyCode.Escape))
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
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        mapsPanelUI.SetActive(false); // Pastikan panel Maps ikut tertutup
        Time.timeScale = 1f; // Kembalikan waktu normal
        isPaused = false;
    }

    public void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f; // Hentikan waktu
        isPaused = true;
    }

    public void ShowMapsPanel()
    {
        pauseMenuUI.SetActive(false); // Sembunyikan panel Pause Menu
        mapsPanelUI.SetActive(true); // Tampilkan panel Maps
    }

    public void BackToPauseMenu()
    {
        mapsPanelUI.SetActive(false); // Sembunyikan panel Maps
        pauseMenuUI.SetActive(true); // Tampilkan kembali panel Pause Menu
    }

    public void QuitGame()
    {
        Debug.Log("Quit Game function executed!");
        Application.Quit(); // Berfungsi hanya di build, tidak di editor
    }
}
