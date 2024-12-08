using UnityEngine;
using UnityEngine.SceneManagement;

public class TeleportButtonManager : MonoBehaviour
{
   public void TeleportToGreenhouse()
    {
        SceneManager.LoadScene("GreenHouse");   // Pindah ke scene transisi awan
    }
}
