using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Vector3 restartPoint; // Menyimpan posisi restart point

    private void Start()
    {
        // Set posisi awal sebagai restart point
        restartPoint = transform.position;
    }

    public void SetRestartPoint(Vector3 newPoint)
    {
        // Perbarui posisi restart point
        restartPoint = newPoint;
    }

    public void TeleportToRestartPoint()
    {
        // Pindahkan player ke restart point
        transform.position = restartPoint;
        Debug.Log("Player teleported to restart point: " + restartPoint);
    }
}
