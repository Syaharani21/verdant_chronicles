using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenuUI;  // Panel untuk pause menu
    private bool isPaused = false;  // Status apakah game sedang di-pause

    void Update()
    {
        // Cek jika tombol ESC ditekan
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("Escape Key Pressed"); 
            // Jika game sudah di-pause, resume game
            if (isPaused)
            {
                Resume();
            }
            // Jika game belum di-pause, pause game
            else
            {
                Pause();
            }
        }
    }

    // Fungsi untuk melanjutkan permainan
    public void Resume()
    {
        Debug.Log("Game Resumed"); // Logging untuk pengecekan
        pauseMenuUI.SetActive(false);  // Sembunyikan pause menu
        Time.timeScale = 1f;  // Kembalikan waktu ke normal
        isPaused = false;  // Tandai bahwa game tidak di-pause
    }

    // Fungsi untuk menghentikan sementara (pause) game
    void Pause()
    {
        Debug.Log("Game Paused");  // Logging untuk pengecekan
        pauseMenuUI.SetActive(true);  // Tampilkan pause menu
        Time.timeScale = 0f;  // Hentikan waktu di dalam game
        isPaused = true;  // Tandai bahwa game sedang di-pause
    }

    // Fungsi untuk tombol "Continue"
    public void OnContinueButtonClicked()
    {
        Resume();  // Panggil fungsi Resume saat tombol diklik
    }

    // Fungsi untuk tombol "Quit"
    public void OnQuitButtonClicked()
    {
        Debug.Log("Quitting Game...");  // Logging untuk pengecekan
        Application.Quit();  // Keluar dari aplikasi (hanya berfungsi di build, tidak di editor)
    }
}
