using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    public RectTransform cloudPanel;  // Panel awan untuk transisi
    public float cloudSpeed = 1000f;  // Kecepatan gerakan awan
    public string targetScene;        // Nama scene tujuan

    private bool isTransitioning = false;

    void Start()
    {
        // Pastikan awan dimulai di luar layar
        cloudPanel.anchoredPosition = new Vector2(-Screen.width, 0);
    }

    public void StartTransition(string sceneName)
    {
        if (!isTransitioning)
        {
            isTransitioning = true;
            targetScene = sceneName;
            StartCoroutine(TransitionIn());
        }
    }

    IEnumerator TransitionIn()
    {
        while (cloudPanel.anchoredPosition.x < 0)
        {
            cloudPanel.anchoredPosition += Vector2.right * cloudSpeed * Time.deltaTime;
            yield return null;
        }

        yield return LoadScene();
    }

    IEnumerator LoadScene()
    {
        SceneManager.LoadScene("GreenHouse");

        // Tunggu beberapa frame agar scene selesai dimuat
        yield return new WaitForSeconds(0.5f);

        // Transisi keluar
        StartCoroutine(TransitionOut());
    }

    IEnumerator TransitionOut()
    {
        while (cloudPanel.anchoredPosition.x < Screen.width)
        {
            cloudPanel.anchoredPosition += Vector2.right * cloudSpeed * Time.deltaTime;
            yield return null;
        }

        isTransitioning = false;
    }
}
