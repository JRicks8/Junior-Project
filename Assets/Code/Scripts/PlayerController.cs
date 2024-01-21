using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngineInternal;

public class PlayerController : MonoBehaviour
{
    [Header("Object References")]
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Camera cam;
    [SerializeField] private Transform bottom;

    [Header("Input")]
    [SerializeField] private InputActionAsset actions;
    private InputAction moveAction;
    private InputAction lookAction;

    [Header("Camera")]
    [SerializeField] private Transform CameraFocusPoint;
    [SerializeField] private Vector2 cameraRotation = Vector2.zero;
    [SerializeField] private float cameraDistance; // Where x is the horizontal offset and y is the vertical offset
    [SerializeField] private float cameraHitCushion; // When raycasting to detect terrain we need to account for some extra distance so the camera doesn't clip into the terrain
    [SerializeField][Range(0.1f, 2.0f)] private float cameraSensitivity;
    [SerializeField] private float yRotationLimit = 88.0f;
    [SerializeField] private LayerMask CameraBlocking;

    [Header("Movement")]
    [SerializeField] private LayerMask whatIsGround;
    [SerializeField] private float maxWalkSpeed;
    [SerializeField] private float maxRunSpeed;
    [SerializeField] private float acceleration;
    [SerializeField][Range(0.01f, 1.0f)] private float groundIdleDrag;
    [SerializeField][Range(0.01f, 1.0f)] private float airIdleDrag;

    [SerializeField] private float turnSpeedMin = 0.05f;
    [SerializeField] private float turnSpeedMax = 0.5f;
    [SerializeField][Range(0.0f, 2.0f)] private float turnSpeed; // The rate at which the character will rotate to face the desired direction

    [SerializeField] private Vector2 moveInput;
    [SerializeField] private Vector3 facingDirection = Vector3.forward;
    [SerializeField] private Vector3 desiredDirection = Vector3.forward;

    [SerializeField] private bool walking = true;
    [SerializeField] private bool running = false;
    [SerializeField] private bool grounded = false;

    [Header("Details")]
    public GameObject currentGround;

    private void Awake()
    {
        // Input
        InputActionMap actionMap = actions.FindActionMap("Gameplay");
        actionMap.FindAction("jump").performed += OnJumpAction;
        actionMap.FindAction("crouch").performed += OnCrouchAction;
        actionMap.FindAction("interact").performed += OnInteractAction;
        moveAction = actionMap.FindAction("move");
        lookAction = actionMap.FindAction("look");

        // Objects
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {

    }

    private void Update()
    {
        Vector2 lookInput = lookAction.ReadValue<Vector2>();
        moveInput = moveAction.ReadValue<Vector2>();

        CameraUpdate(lookInput);

        Debug.DrawLine(transform.position, transform.position + facingDirection * 2, Color.blue);
        Debug.DrawLine(transform.position, transform.position + desiredDirection * 2, Color.red);
    }

    private void FixedUpdate()
    {
        CheckGround();

        Move();
    }

    /// <summary>
    /// Should be called in Fixed Update.
    /// </summary>
    private void Move()
    {
        // Setup Variables
        Vector3 horizVelocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        float horizVelocityMag = horizVelocity.magnitude;

        Vector3 camForward = cam.transform.forward;
        Vector3 flatCamForward = new Vector3(camForward.x, 0, camForward.z).normalized;
        Vector3 camRight = cam.transform.right;
        Vector3 flatCamRight = new Vector3(camRight.x, 0, camRight.z).normalized;

        // Update Turning Speed
        float t = Mathf.Clamp01(1 - (horizVelocityMag / maxRunSpeed));
        turnSpeed = turnSpeedMin + (turnSpeedMax - turnSpeedMin) * t;

        if (moveInput.x != 0 || moveInput.y != 0) // If we are moving
        {
            if (grounded) // Ground movement calculations
            {
                // Calculate the desired movement direction
                Vector3 rightForce = flatCamRight * moveInput.x;
                Vector3 forwardForce = flatCamForward * moveInput.y;
                desiredDirection = (rightForce + forwardForce).normalized;

                // Try to align the facing direction with the desired move direction
                facingDirection = Vector3.RotateTowards(facingDirection, desiredDirection, turnSpeed, 1.0f).normalized;

                // Move using the facing direction
                Vector3 moveForce = facingDirection * acceleration;
                rb.AddForce(moveForce, ForceMode.Acceleration);
            }
            else // Air movement calculations
            {
                // TODO: Air Movement
            }
        }
        else if (grounded) // We aren't moving and we're on the ground, so stifle the velocity to imitate drag
        {
            rb.velocity *= groundIdleDrag;
        }

        // Limit maximum horizontal velocity
        if (walking && horizVelocityMag > maxWalkSpeed)
        {
            Debug.Log(horizVelocityMag);
            LimitHorizontalVelocity(horizVelocity, maxWalkSpeed);
        }
        else if (running && horizVelocityMag > maxRunSpeed)
        {
            LimitHorizontalVelocity(horizVelocity, maxRunSpeed);
        }
    }

    private void CheckGround()
    {
        Collider[] colliders = new Collider[1];
        Physics.OverlapSphereNonAlloc(bottom.position, 0.3f, colliders, whatIsGround);
        if (colliders[0] != null)
        {
            grounded = true;
            currentGround = colliders[0].gameObject;
        }
        else
        {
            grounded = false;
            currentGround = null;
        }
    }

    /// <summary>
    /// Should be called in Update. Updates the Camera position and rotation, accounting for mouse/stick input and blocking terrain.
    /// </summary>
    /// <param name="lookDelta"></param>
    private void CameraUpdate(Vector2 lookDelta)
    {
        Transform camTransform = cam.transform;

        // Handle Rotation
        cameraRotation.x += lookDelta.x * cameraSensitivity;
        cameraRotation.y += lookDelta.y * cameraSensitivity;
        cameraRotation.y = Mathf.Clamp(cameraRotation.y, -yRotationLimit, yRotationLimit);
        Quaternion xQuat = Quaternion.AngleAxis(cameraRotation.x, Vector3.up);
        Quaternion yQuat = Quaternion.AngleAxis(cameraRotation.y, Vector3.left);
        camTransform.localRotation = xQuat * yQuat;

        // Handle Position
        // Position is dependent on the new rotation of the camera, so we do this second.
        float newCameraDistance;
        Ray camRay = new Ray(CameraFocusPoint.position, camTransform.forward * -1);
        if (Physics.Raycast(camRay, out RaycastHit hit, cameraDistance + cameraHitCushion, CameraBlocking))
        {
            newCameraDistance = hit.distance;
        }
        else
        {
            newCameraDistance = cameraDistance;
        }
        Vector3 newPosition = CameraFocusPoint.position - camTransform.forward * (newCameraDistance - cameraHitCushion);
        camTransform.position = newPosition;
    }

    private void LimitHorizontalVelocity(Vector3 horizontalVel, float magLimit)
    {
        Vector3 desiredHorizVel = horizontalVel.normalized * magLimit;
        Vector3 newVelocity = new Vector3(
            desiredHorizVel.x + Mathf.Sqrt(rb.velocity.x - horizontalVel.x),
            rb.velocity.y,
            desiredHorizVel.z + Mathf.Sqrt(rb.velocity.z - horizontalVel.z));
        Debug.Log(newVelocity);
        rb.velocity = newVelocity;
    }

    // Input Action Callbacks
    private void OnJumpAction(InputAction.CallbackContext context)
    {

    }

    private void OnCrouchAction(InputAction.CallbackContext context)
    {

    }

    private void OnInteractAction(InputAction.CallbackContext context)
    {

    }

    void OnEnable()
    {
        actions.FindActionMap("gameplay").Enable();
    }
    void OnDisable()
    {
        actions.FindActionMap("gameplay").Disable();
    }
}
