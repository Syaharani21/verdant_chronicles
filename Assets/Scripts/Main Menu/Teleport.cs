using UnityEngine;
using UnityEngine.SceneManagement;

public class Teleport : MonoBehaviour
{
    public void TeleportToNusendra()
    { Debug.Log("Attempting to load scene 'Nusendra'");
    Time.timeScale = 1f;
    SceneManager.LoadScene("Level2");
}
}