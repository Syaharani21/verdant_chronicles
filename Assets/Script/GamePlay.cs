using UnityEngine;

public class GamePlay : MonoBehaviour
{
    public GameObject gameplayCanvas; // Canvas untuk gameplay (drag & drop di Inspector)
    public GameObject pauseMenuCanvas; // Canvas untuk pause menu (drag & drop di Inspector)
    
    private bool isPaused = false;

    void Start()
    {
        // Awalnya, pause menu disembunyikan
        pauseMenuCanvas.SetActive(false);
    }

    void Update()
    {
        // Jika tombol ESC ditekan
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                ResumeGame(); // Kembali ke gameplay
            }
            else
            {
                PauseGame(); // Tampilkan pause menu
            }
        }
    }

    // Method untuk menampilkan pause menu
    void PauseGame()
    {
        pauseMenuCanvas.SetActive(true); // Tampilkan canvas pause
        gameplayCanvas.SetActive(false); // Sembunyikan canvas gameplay
        Time.timeScale = 0f; // Hentikan waktu
        isPaused = true;
    }

    // Method untuk kembali ke gameplay
    public void ResumeGame()
    {
        pauseMenuCanvas.SetActive(false); // Sembunyikan canvas pause
        gameplayCanvas.SetActive(true); // Tampilkan canvas gameplay
        Time.timeScale = 1f; // Lanjutkan waktu
        isPaused = false;
    }

    // Opsional: method untuk keluar dari game (quit)
    public void QuitGame()
    {
        Debug.Log("Quitting game...");
        Application.Quit(); // Untuk keluar dari game saat build
    }
}
