using UnityEngine;

public class CameraController : MonoBehaviour
{
    // Room camera
    [SerializeField] private float speed;
    private float currentPosX;
    private Vector3 velocity = Vector3.zero;

    // Follow player
    [SerializeField] private Transform player;
    [SerializeField] private float aheadDistance;
    [SerializeField] private float cameraSpeed;
    [SerializeField] private float dashCameraSpeed = 0.5f;
    private float targetCameraSpeed;
    private float lookAhead;

    // Access PlayerMovement to check dash status
    private PlayerMovement playerMovement;

    private void Awake()
    {
        // Set camera z position
        transform.position = new Vector3(transform.position.x, transform.position.y, -10f);
        playerMovement = player.GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        // Smooth transition for room camera movement
        transform.position = Vector3.SmoothDamp(
            transform.position,
            new Vector3(currentPosX, transform.position.y, transform.position.z),
            ref velocity,
            speed
        );

        // Set camera speed based on dash status
        targetCameraSpeed = playerMovement.IsDashing ? dashCameraSpeed : cameraSpeed;

        // Update camera position
        if (player != null)
        {
            transform.position = Vector3.Lerp(
                transform.position,
                new Vector3(player.position.x + lookAhead, transform.position.y, transform.position.z),
                Time.deltaTime * targetCameraSpeed
            );

            // Adjust lookAhead based on player's scale and dash status
            lookAhead = Mathf.Lerp(lookAhead, (aheadDistance * player.localScale.x), Time.deltaTime * targetCameraSpeed);
        }
    }

    public void MoveToNewRoom(Transform _newRoom)
    {
        currentPosX = _newRoom.position.x;
    }
}
