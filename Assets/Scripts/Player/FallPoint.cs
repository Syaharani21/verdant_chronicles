using UnityEngine;

public class FallPoint : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) // Pastikan objek player memiliki tag "Player"
        {
            // Teleportasikan player ke restart point
            collision.GetComponent<PlayerController>().TeleportToRestartPoint();
            Debug.Log("Player fell and teleported back to restart point.");
        }
    }
}
