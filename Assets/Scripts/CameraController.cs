using UnityEngine;

public class CameraController : MonoBehaviour
{
    // Room camera
    [SerializeField] private float speed;
    private float currentPosX;
    private Vector3 velocity = Vector3.zero;

    // Follow player
    [SerializeField] private Transform player;
    [SerializeField] private float aheadDistance = 8f;
    [SerializeField] private float cameraSpeed;
    [SerializeField] private float dashCameraSpeed = 1f;
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
            lookAhead = Mathf.Lerp(lookAhead, (aheadDistance * player.localScale.x), Time.deltaTime * targetCameraSpeed);

            float targetCameraPosX = player.position.x + lookAhead;

            if (playerMovement.IsDashing)
            {
                targetCameraPosX = Mathf.Lerp(transform.position.x, player.position.x + lookAhead, targetCameraSpeed);
            }

            transform.position = Vector3.Lerp(
                transform.position,
                new Vector3(targetCameraPosX, transform.position.y, transform.position.z),
                Time.deltaTime * targetCameraSpeed
            );
        }
    }

    public void MoveToNewRoom(Transform _newRoom)
    {
        currentPosX = _newRoom.position.x;
    }
}
