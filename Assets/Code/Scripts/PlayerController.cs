using System;
using System.Collections;
using System.Linq.Expressions;
using TMPro;
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

    [Header("Movement Settings")]
    [SerializeField] private LayerMask whatIsGround;
    [SerializeField] private float softMaxWalkSpeed;
    [SerializeField] private float softMaxRunSpeed;
    [SerializeField][Range(0.0f, 0.95f)] private float velocityDecayRate; // If the velocity's mag is more than the limit, multiply the excess by this multiplier to stifle it
    [SerializeField] private float groundAcceleration;
    [SerializeField] private float airAcceleration;
    [SerializeField][Range(0.01f, 1.0f)] private float groundIdleDrag;
    [SerializeField][Range(0.01f, 1.0f)] private float airIdleDrag;
    [SerializeField] private float jumpUpForce;
    [SerializeField] private float jumpForwardForce;
    [SerializeField] private int maxNumJumps;

    [SerializeField] private float gravity;
    [SerializeField] private float gravityScale;

    [SerializeField] private float turnSpeedMin;
    [SerializeField] private float turnSpeedMax;
    [SerializeField] private float walkingTurnSpeedMulti = 3.0f;

    [Header("Movement Details")]
    [SerializeField][Range(0.01f, 5.0f)] private float turnSpeed; // The rate at which the character will rotate to face the desired direction
    [SerializeField] private float tempMaxAirMag; // Need to limit air velocity, this is reset every time the player jumps to their velocity at the start of the jump or when the player does an action in mid-air that effects the velocity

    [SerializeField] private Vector2 moveInput;
    [SerializeField] private Vector3 facingDirection = Vector3.forward; // The flat vector representing the direction the player is facing and moving in.
    [SerializeField] private Vector3 desiredDirection = Vector3.forward; // The flat vector representing the direction the player wants to go.

    [SerializeField] private bool walking = true;
    [SerializeField] private bool sprinting = false;
    [SerializeField] private bool grounded = false;

    [SerializeField] private int jumps;

    [Header("General Details")]
    public GameObject currentGround;

    private IEnumerator regainJumpsAfterDelay;

    private void Awake()
    {
        // Input
        InputActionMap actionMap = actions.FindActionMap("Gameplay");
        actionMap.FindAction("jump").performed += OnJumpAction;
        actionMap.FindAction("sprint").started += OnSprintStart;
        actionMap.FindAction("sprint").canceled += OnSprintCanceled;
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

    float airTime = 0.0f;
    private void Update()
    {
        if (!grounded) airTime += Time.deltaTime;
        else if (grounded && airTime > 0.0f)
        {
            Debug.Log("Air Time: " + airTime);
            airTime = 0.0f;
        }
        Vector2 lookInput = lookAction.ReadValue<Vector2>();
        moveInput = moveAction.ReadValue<Vector2>();

        CameraUpdate(lookInput);

        Debug.DrawLine(transform.position, transform.position + facingDirection * 2, Color.blue);
        Debug.DrawLine(transform.position, transform.position + desiredDirection * 2, Color.red);
        DebugUpdate();
    }

    private void FixedUpdate()
    {
        rb.AddForce(new Vector3(0, gravity * gravityScale, 0), ForceMode.Acceleration);

        CheckGround();

        Move();
    }

    /// <summary>
    /// Should be called in Fixed Update.
    /// </summary>
    private void Move()
    {
        // Setup Variables
        Vector3 flatVelocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        float flatVelocityMag = flatVelocity.magnitude;

        Vector3 camForward = cam.transform.forward;
        Vector3 flatCamForward = new Vector3(camForward.x, 0, camForward.z).normalized;
        Vector3 camRight = cam.transform.right;
        Vector3 flatCamRight = new Vector3(camRight.x, 0, camRight.z).normalized;

        // Update Turning Speed
        float t = Mathf.Clamp01(1 - (flatVelocityMag / softMaxRunSpeed));
        turnSpeed = turnSpeedMin + (turnSpeedMax - turnSpeedMin) * t;
        if (walking && grounded)
            turnSpeed *= walkingTurnSpeedMulti;

        // Determine if we're trying to move
        bool tryingToMove = moveInput.x != 0 || moveInput.y != 0;
        if (tryingToMove) // If we are trying to move
        {
            // Calculate the desired movement direction
            Vector3 rightForce = flatCamRight * moveInput.x;
            Vector3 forwardForce = flatCamForward * moveInput.y;
            desiredDirection = (rightForce + forwardForce).normalized;

            if (grounded) // Ground movement calculations
            {
                // Try to align the facing direction with the desired move direction
                facingDirection = Vector3.RotateTowards(facingDirection, desiredDirection, turnSpeed, 1.0f).normalized;

                // Move using the facing direction
                Vector3 moveForce = facingDirection * groundAcceleration;
                rb.AddForce(moveForce, ForceMode.Acceleration);

                // Limit flat velocity if we're trying to move and grounded.
                // We don't limit it this way if we're not moving because the idle drag should decrease the velocity much faster
                // than this method anyways.
                if (walking && flatVelocityMag > softMaxWalkSpeed)
                {
                    LimitFlatVelocitySoft(flatVelocity, softMaxWalkSpeed);
                }
                else if (sprinting && flatVelocityMag > softMaxRunSpeed)
                {
                    LimitFlatVelocitySoft(flatVelocity, softMaxRunSpeed);
                }
            }
            else // Air movement calculations
            {
                // Facing direction should remain unchanged
                // Allow limited movement, but the player shouldn't be able to increase their flat velocity by just moving

                // Move using the desired direction
                Vector3 moveForce = desiredDirection * airAcceleration;
                rb.AddForce(moveForce, ForceMode.Acceleration);

                // Limit flat velocity if its greater than the max air magnitude
                if (flatVelocityMag > tempMaxAirMag)
                {
                    LimitFlatVelocitySoft(flatVelocity, tempMaxAirMag);
                }
            }
        }
        else if (grounded) // We aren't trying to move and we're on the ground, so stifle the flat velocity to imitate drag
        {
            rb.velocity = new Vector3(rb.velocity.x * groundIdleDrag, rb.velocity.y, rb.velocity.z * groundIdleDrag);
        }
        else // We aren't trying to move and we're in the air, so stifle the flat velocity slightly using the air idle drag
        {
            rb.velocity = new Vector3(rb.velocity.x * airIdleDrag, rb.velocity.y, rb.velocity.z * airIdleDrag);
        }
    }

    private void Jump()
    {
        if (jumps <= 0) return;
        jumps--;

        rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        Vector3 compositeForce = desiredDirection * jumpForwardForce;
        compositeForce.y += jumpUpForce;
        rb.AddForce(compositeForce * rb.mass, ForceMode.Impulse);
        tempMaxAirMag = Mathf.Max(rb.velocity.magnitude, softMaxWalkSpeed);
    }

    private void CheckGround()
    {
        Collider[] colliders = new Collider[1];
        Physics.OverlapSphereNonAlloc(bottom.position, 0.3f, colliders, whatIsGround);
        if (colliders[0] != null)
        {
            if (!grounded) OnLand();
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

    /// <summary>
    /// Soft limit for the flat velocity (x and z components). Uses the velocity decay rate to calculate
    /// the new velocity, somewhere between the desired velocity and actual velocity.
    /// </summary>
    /// <param name="flatVelocity"></param>
    /// <param name="magLimit"></param>
    private void LimitFlatVelocitySoft(Vector3 flatVelocity, float magLimit)
    {
        Vector3 desiredFlatVel = flatVelocity.normalized * magLimit;
        Vector3 newVelocity = new Vector3(
            desiredFlatVel.x + (flatVelocity.x - desiredFlatVel.x) * velocityDecayRate,
            rb.velocity.y,
            desiredFlatVel.z + (flatVelocity.z - desiredFlatVel.z) * velocityDecayRate);
        rb.velocity = newVelocity;
    }

    private void OnLand()
    {
        regainJumpsAfterDelay = RegainJumpsAfterDelay(0.06f);
        StartCoroutine(regainJumpsAfterDelay);
    }

    private bool active = false;
    /// <summary>
    /// This IEnumerator replenishes jumps after a short delay.
    /// Intended to prevent potentially infinite velocity gain by jumping within a couple of frames after hitting the ground.
    /// </summary>
    /// <param name="delay"></param>
    /// <returns></returns>
    IEnumerator RegainJumpsAfterDelay(float delay)
    {
        if (active) yield break;
        active = true;
        yield return new WaitForSeconds(delay);
        active = false;
        jumps = maxNumJumps;
    }

    // Input Action Callbacks
    private void OnJumpAction(InputAction.CallbackContext context)
    {
        Jump();
    }

    private void OnSprintStart(InputAction.CallbackContext context)
    {
        walking = false;
        sprinting = true;
    }

    private void OnSprintCanceled(InputAction.CallbackContext context)
    {
        walking = true;
        sprinting = false;
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

    // Debug
    [Header("Debug")]
    public TextMeshProUGUI speedText;
    private void DebugUpdate()
    {
        if (speedText != null)
        {
            Vector3 flatVelocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
            decimal value = Math.Round((decimal)flatVelocity.magnitude, 3);
            speedText.text = "Speed: " + value;
        }
    }
}
