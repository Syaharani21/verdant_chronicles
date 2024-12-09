using UnityEngine;

public class RestartPoint : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) // Pastikan objek player memiliki tag "Player"
        {
            // Ambil skrip PlayerController dan set posisi restart point
            collision.GetComponent<PlayerController>().SetRestartPoint(transform.position);
            Debug.Log("Restart point updated: " + transform.position);
        }
    }
}
