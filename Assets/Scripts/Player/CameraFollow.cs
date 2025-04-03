using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target; // The player's transform
    public Vector3 offset = new Vector3(0f, 30f, 20f); // Camera offset from the player
    public float smoothSpeed = 0.2f; // How smoothly the camera follows
    public float jumpSwayAmount = 0.2f; // How much the camera sways when jumping
    public float swaySmoothness = 0.1f; // How smooth the sway effect is

    // Camera orbit settings
    public float rotationSpeed = 5f;
    public float minVerticalAngle = -80f;
    public float maxVerticalAngle = 80f;
    public float orbitDistance = 5f;
    public float minDistance = 30f;
    public float maxDistance = 100f;
    public float zoomSpeed = 10f;

    private Vector3 currentVelocity;
    private float verticalSway;
    private Rigidbody targetRigidbody;
    private float currentRotationX;
    private float currentRotationY;
    private float currentDistance;

    void Start()
    {
        // Try to find the player if target is not assigned
        if (target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                target = player.transform;
            }
            else
            {
                Debug.LogError("CameraFollow: No target assigned and no GameObject with 'Player' tag found!");
                return;
            }
        }

        // Get the player's Rigidbody component
        targetRigidbody = target.GetComponent<Rigidbody>();
        if (targetRigidbody == null)
        {
            Debug.LogError("CameraFollow: Target does not have a Rigidbody component!");
        }

        // Initialize camera rotation and distance
        currentRotationX = transform.eulerAngles.y;
        currentRotationY = transform.eulerAngles.x;
        currentDistance = orbitDistance;
    }

    void LateUpdate()
    {
        if (target == null || targetRigidbody == null)
            return;

        // Handle right-click camera orbit
        if (Input.GetMouseButton(1)) // Right mouse button
        {
            currentRotationX += Input.GetAxis("Mouse X") * rotationSpeed;
            currentRotationY -= Input.GetAxis("Mouse Y") * rotationSpeed;
            currentRotationY = Mathf.Clamp(currentRotationY, minVerticalAngle, maxVerticalAngle);
        }

        // Handle mouse wheel zoom
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        currentDistance = Mathf.Clamp(currentDistance - scroll * zoomSpeed, minDistance, maxDistance);

        // Calculate vertical sway based on player's vertical velocity
        float targetSway = targetRigidbody.linearVelocity.y * jumpSwayAmount;
        verticalSway = Mathf.Lerp(verticalSway, targetSway, swaySmoothness);

        // Calculate the desired position with orbit
        Quaternion rotation = Quaternion.Euler(currentRotationY, currentRotationX, 0);
        Vector3 desiredPosition = target.position + rotation * new Vector3(0, 0, -currentDistance) + new Vector3(0, verticalSway, 0);
        
        // Smoothly move the camera to the desired position
        transform.position = Vector3.SmoothDamp(
            transform.position,
            desiredPosition,
            ref currentVelocity,
            smoothSpeed
        );

        // Make the camera look at the player
        transform.LookAt(target.position + new Vector3(0, verticalSway, 0));
    }
} 