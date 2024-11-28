using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CloudTransition : MonoBehaviour
{
   public RectTransform cloudPanel;  // Panel awan (UI element)
    public float cloudSpeed = 1000f;  // Kecepatan gerakan awan
    public string targetScene;        // Nama scene tujuan

    private bool isTransitioningIn = false;
    private bool isTransitioningOut = false;

    void Start()
    {
        // Pastikan awan dimulai di luar layar
        cloudPanel.anchoredPosition = new Vector2(-Screen.width, 0);
        isTransitioningIn = true;  // Mulai transisi masuk
    }

    void Update()
    {
        if (isTransitioningIn)
        {
            // Gerakan awan masuk
            cloudPanel.anchoredPosition += Vector2.right * cloudSpeed * Time.deltaTime;

            if (cloudPanel.anchoredPosition.x >= 0)  // Awan menutupi layar
            {
                isTransitioningIn = false;
                StartCoroutine(LoadNextScene());
            }
        }
        else if (isTransitioningOut)
        {
            // Gerakan awan keluar
            cloudPanel.anchoredPosition += Vector2.right * cloudSpeed * Time.deltaTime;

            if (cloudPanel.anchoredPosition.x >= Screen.width)  // Awan keluar layar
            {
                isTransitioningOut = false;
            }
        }
    }

    IEnumerator LoadNextScene()
    {
        // Tunggu sebentar sebelum memuat scene
        yield return new WaitForSeconds(1f);

        // Muat scene berikutnya
        SceneManager.LoadScene("Nusendra");

        // Setelah scene baru dimuat, awan bergerak keluar
        isTransitioningOut = true;
    }
}
