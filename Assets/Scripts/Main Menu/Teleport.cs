using UnityEngine;
using UnityEngine.SceneManagement;

public class Teleport : MonoBehaviour
{

    public void OnSceneChangeButtonClick()
    {
         SceneManager.LoadScene("Level2");
    }
}
