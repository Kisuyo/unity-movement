using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float jumpForce = 5f;
    public float movementThreshold = 0.1f; // Threshold for movement detection
    public float groundCheckDistance = 0.2f;
    public float groundStickForce = 10f; // Force to keep character grounded

    private Animator animator;
    private Rigidbody rb;
    private bool isGrounded;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        
        // Basic Rigidbody setup
        if (rb != null)
        {
            rb.freezeRotation = true;
            rb.useGravity = true;
            rb.interpolation = RigidbodyInterpolation.Interpolate; // Smoother movement
        }
    }

    void Update()
    {
        // Get input axes and invert them
        float horizontal = -Input.GetAxis("Horizontal");  // Inverted horizontal
        float vertical = -Input.GetAxis("Vertical");      // Inverted vertical

        // Calculate movement direction
        Vector3 moveDirection = new Vector3(horizontal, 0, vertical).normalized;
        float currentSpeed = moveDirection.magnitude;

        // Check if we're moving (using raw input for faster response)
        bool isMoving = Mathf.Abs(horizontal) > movementThreshold || Mathf.Abs(vertical) > movementThreshold;

        // Update animation state continuously
        if (animator != null)
        {
            animator.SetFloat("Speed", isMoving ? 1f : 0f);
        }

        // Move the player
        if (currentSpeed > movementThreshold)
        {
            Vector3 movement = moveDirection * moveSpeed;
            rb.linearVelocity = new Vector3(movement.x, rb.linearVelocity.y, movement.z);
        }
        else
        {
            rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
        }

        // Jump
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    void FixedUpdate()
    {
        // Check if grounded
        isGrounded = Physics.Raycast(transform.position, Vector3.down, groundCheckDistance);

        // Apply downward force when grounded to prevent floating
        if (isGrounded)
        {
            rb.AddForce(Vector3.down * groundStickForce, ForceMode.Force);
        }
    }

    private void OnDrawGizmos()
    {
        // Visualize ground check ray
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, Vector3.down * groundCheckDistance);
    }
}
