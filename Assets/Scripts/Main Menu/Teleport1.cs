using UnityEngine;
using UnityEngine.SceneManagement;

public class TeleportButtonManager : MonoBehaviour
{
   public void TeleportToGreenhouse()
    { Debug.Log("Attempting to load scene 'Nusendra'");
    Time.timeScale = 1f;
    SceneManager.LoadScene("GreenHouse");
}}
