using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private float speed;
    private float currentPosX;
    private Vector3 velocity = Vector3.zero;

    [SerializeField] private Transform player;
    [SerializeField] private float dashCameraSpeed = 1f;
    public float FollowSpeed = 2f;
    public float yOffset = 1f;
    private float targetCameraSpeed;
    private float lookAhead;

    private PlayerMovement playerMovement;
    
    private float idleTimer = 0f;  // Timer for idle state
    private bool isIdle = false;  // Flag to check if player is idle

    private void Awake()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, -10f);
        playerMovement = player.GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        targetCameraSpeed = playerMovement.IsDashing ? dashCameraSpeed : FollowSpeed;

        if (player != null)
        {
            lookAhead = Mathf.Lerp(lookAhead, 1f * player.localScale.x, Time.deltaTime * targetCameraSpeed);

            float targetCameraPosX = player.position.x + lookAhead;

            if (playerMovement.IsDashing)
            {
                targetCameraPosX = Mathf.Lerp(transform.position.x, player.position.x + lookAhead, targetCameraSpeed);
            }

            Vector3 newPos = new Vector3(targetCameraPosX, player.position.y + yOffset, -10f);

            transform.position = Vector3.Slerp(transform.position, newPos, FollowSpeed * Time.deltaTime);

            // Check if player is idle
            if (playerMovement.IsDashing || Mathf.Abs(playerMovement.GetComponent<Rigidbody2D>().velocity.x) > 0.1f)
            {
                idleTimer = 0f;  // Reset idle timer if moving
                isIdle = false;
            }
            else
            {
                idleTimer += Time.deltaTime;  // Increase idle timer if idle
                if (idleTimer >= 1f && !isIdle)
                {
                    isIdle = true;
                    // Set camera to center on player after 1 second of idling
                    currentPosX = player.position.x;
                }
            }
        }

        // Smooth movement of the camera in the x-axis when changing rooms
        transform.position = Vector3.SmoothDamp(
            transform.position,
            new Vector3(currentPosX, transform.position.y, transform.position.z),
            ref velocity,
            speed
        );
    }

    public void MoveToNewRoom(Transform _newRoom)
    {
        currentPosX = _newRoom.position.x;
    }
}
