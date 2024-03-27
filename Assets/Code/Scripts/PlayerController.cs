using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour, IDataPersistence
{
    public enum Abilities 
    {
        DoubleJump,
        Dash,
        Dive,
        Grapple,
    }

    [Header("Object References")]
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Camera cam;
    [SerializeField] private Transform bottom;
    [SerializeField] private GrappleVisual grappleVisualScript;

    [Header("Input")]
    [SerializeField] private InputActionAsset actions;
    private InputAction moveAction;
    private InputAction jumpAction;
    private InputAction lookAction;
    [SerializeField] private float bufferInputThres;
    private float bufferInputTimer;
    private string lastBufferedAction;
    private string bufferedAction;

    [Header("Camera")]
    [SerializeField] private Transform CameraFocusPoint;
    [SerializeField] private Vector2 cameraRotation = Vector2.zero;
    [SerializeField] private float cameraDistance; // Where x is the horizontal offset and y is the vertical offset
    [SerializeField] private float cameraHitCushion; // When raycasting to detect terrain we need to account for some extra distance so the camera doesn't clip into the terrain
    [SerializeField][Range(0.1f, 2.0f)] private float cameraSensitivity;
    [SerializeField] private float yRotationLimit = 88.0f;
    [SerializeField] private LayerMask CameraBlocking;
    private bool isInHermesSection = true;

    [Header("Interact")]
    [SerializeField] private float interactRange;

    [Space]
    [Header("-----Movement Settings-----")]
    [Header("Change these settings to effect the movement.")]
    [Header("Walking & Running")]
    [SerializeField] private LayerMask whatIsGround;
    [SerializeField] private float softMaxRunSpeed;
    [SerializeField] private float groundAcceleration;
    [SerializeField] private float airAcceleration;
    [SerializeField][Range(0.01f, 1.0f)] private float groundIdleDrag;
    [SerializeField][Range(0.01f, 1.0f)] private float airIdleDrag;
    [SerializeField][Range(0.0f, 1.0f)] private float slopeLimit; // Dot value. 1.0f is no limit to slope, 0.0f means you can't walk on any surface basically.
    [Header("Jumping")]
    [SerializeField] private bool hasDoubleJump;
    [SerializeField] private float jumpUpForce;
    [SerializeField] private float jumpForwardForce;
    private int maxNumJumps;
    [Header("Dashing")]
    [SerializeField] private bool hasDash;
    [SerializeField] private float dashVelocityMagnitude;
    [SerializeField] private float dashDuration;
    [SerializeField] private float maxAirMagPostDash; // The max air magnitude after dashing
    [SerializeField] private int maxNumDashes;
    [Header("Diving")]
    [SerializeField] private bool hasDive;
    [SerializeField] private float diveMaxDuration; // The dive persists until hitting the ground or the max time is reached
    [SerializeField] private Vector3 diveVelocity;
    [Header("Step Up")]
    [SerializeField] private float stepHeight;
    [SerializeField] private float forwardStepTest;
    [Header("Grapple")]
    [SerializeField] private bool hasGrapple;
    [SerializeField] private float grappleRange;
    [SerializeField] private float grappleMinDistance;
    [SerializeField] private float grappleAcceleration;
    [SerializeField] private float biasToMoveDir; // The grapple should take into account the desired move direction.
    [Header("Other")]
    [SerializeField] private float defaultGravityScale;
    [SerializeField] private float steepSlopeGravityScale; // For when we want to prevent the player from gliding up slopes that are too steep
    private float gravity = -9.81f;

    [SerializeField][Range(0.0f, 0.95f)] private float velocityDecayRate; // If the velocity's mag is more than the limit, multiply the excess by this multiplier to stifle it
    [SerializeField] private float turnSpeedMin;
    [SerializeField] private float turnSpeedMax;
    [SerializeField] private float groundedTurnSpeedMulti;

    [Header("-----End Movement Settings-----")]
    [Space]
    [Header("Movement Details")]
    [SerializeField][Range(0.01f, 5.0f)] private float turnSpeed; // The rate at which the character will rotate to face the desired direction
    [SerializeField] private float tempMaxAirMag; // Need to limit air velocity, this is reset every time the player jumps to their velocity at the start of the jump or when the player does an action in mid-air that effects the velocity
    [SerializeField] private float gravityScale;

    [SerializeField] private Vector2 moveInput;
    [SerializeField] private Vector3 facingDirection = Vector3.forward; // The flat vector representing the direction the player is facing and moving in.
    [SerializeField] private Vector3 desiredDirection = Vector3.forward; // The flat vector representing the direction the player wants to go.

    [SerializeField] private bool busy = false; // Busy is the catch-all for actions. If we are ever "busy", we cannot take any action.
    [SerializeField] private bool grounded = false;
    [SerializeField] private bool jumping = false;
    [SerializeField] private bool dashing = false;
    [SerializeField] private bool diving = false;
    [SerializeField] private bool grappling = false;

    [SerializeField] private int jumpsLeft;
    [SerializeField] private int dashesLeft;

    [Header("General Details")]
    public GameObject currentGround;
    [SerializeField] private GameObject lastGround;
    [SerializeField] private Vector3 lastGroundPosition;
    [SerializeField] private bool hasPackage;

    private IEnumerator dashHandler;
    private IEnumerator diveHandler;
    private IEnumerator grappleHandler;

    public delegate void GenericPlayerControllerDelegate();
    public GenericPlayerControllerDelegate PackageCollected;
    public GenericPlayerControllerDelegate PackageRemoved;

    private void Awake()
    {
        // Input
        InputActionMap actionMap = actions.FindActionMap("Gameplay");
        actionMap.FindAction("dash").performed += OnDashAction;
        actionMap.FindAction("dive").performed += OnDiveAction;
        actionMap.FindAction("interact").performed += OnInteractAction;
        actionMap.FindAction("grapple").started += OnGrappleStart;
        actionMap.FindAction("grapple").canceled += OnGrappleEnd;
        //actionMap.FindAction("escape").performed += OnEscapeAction;
        jumpAction = actionMap.FindAction("jump");
        jumpAction.performed += OnJumpAction;
        moveAction = actionMap.FindAction("move");
        lookAction = actionMap.FindAction("look");

        // Objects
        rb = GetComponent<Rigidbody>();

        isInHermesSection = SceneManager.GetActiveScene().name.Equals("HermesSection");
        if (isInHermesSection)
        {
            cam = Camera.main;
            cam.transform.parent = null;
            cam.transform.SetPositionAndRotation(new Vector3(13.2399998f, 13.29f, 0f), Quaternion.Euler(41.387f, -90.0f, 0.0f));
        }
    }

    private void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        if (hasDoubleJump)
            maxNumJumps = 2;
        else
            maxNumJumps = 1;
    }

    private void Update()
    {
        if (PauseMenu.paused)
            return;

        Vector2 lookInput = lookAction.ReadValue<Vector2>();
        moveInput = moveAction.ReadValue<Vector2>();

        if (!isInHermesSection) CameraUpdate(lookInput);

        // If we're moving, step up. This needs to be in update to prevent catching on stairs too often
        if ((new Vector3(rb.velocity.x, 0, rb.velocity.z).magnitude > 0.1f && grounded)
            || (moveInput != Vector2.zero && grounded))
            StepUp();

        Debug.DrawLine(transform.position, transform.position + facingDirection * 2, Color.blue);
        Debug.DrawLine(transform.position, transform.position + desiredDirection * 2, Color.red);
        DebugUpdate();
    }

    private void FixedUpdate()
    {
        if (PauseMenu.paused)
            return;

        ExecuteBufferedAction();

        gravityScale = defaultGravityScale; // Set before checkground because it might be changed
        CheckGround();

        // Gravity
        rb.AddForce(new Vector3(0, gravity * gravityScale, 0), ForceMode.Acceleration);

        Move();
    }

    /// <summary>
    /// Should be called in Fixed Update.
    /// </summary>
    private void Move()
    {
        if (busy) return;

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
        if (grounded)
            turnSpeed *= groundedTurnSpeedMulti;

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
                if (flatVelocityMag > softMaxRunSpeed)
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

    // Return value is the success of the command
    private bool Jump()
    {
        if (diving && jumpsLeft > 0) // Cancel dive
        {
            StopCoroutine(diveHandler);
            diving = false;
            busy = false;
        }

        if (jumpsLeft <= 0 || busy) return false;
        jumpsLeft--;
        jumping = true;
        grappling = false; // Cancel grapple

        rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        Vector3 compositeForce = desiredDirection * jumpForwardForce;
        facingDirection = desiredDirection;
        compositeForce.y += jumpUpForce;
        rb.AddForce(compositeForce * rb.mass, ForceMode.Impulse);
        CalculateTempAirMaxMagnitude(false);
        return true;
    }

    // Return value is the success of the command
    private bool Dash()
    {
        if (diving && dashesLeft > 0) // Cancel dive
        {
            StopCoroutine(diveHandler);
            diving = false;
            busy = false;
        }

        // Do not dash if we're already dashing or committed to another action
        if (dashing || grappling || busy || dashesLeft <= 0) return false;
        dashing = true;
        busy = true;

        Vector3 velocity = desiredDirection * dashVelocityMagnitude;
        dashHandler = DashHandler(velocity);
        StartCoroutine(dashHandler);

        // Adjust the facing direction
        facingDirection = desiredDirection;
        dashesLeft--;

        // If at the beginning of the dash we are grounded, refresh our dashes.
        if (grounded)
            dashesLeft = maxNumDashes;

        return true;
    }

    // Dive freezes horizontal velocity and moves downward at a fixed velocity. Upon hitting the ground,
    // horizontal velocity is restored and the player destroys nearby objects that can be destroyed.
    private bool Dive()
    {
        if (diving) // Cancel dive
        {
            StopCoroutine(diveHandler);
            diving = false;
            busy = false;
            return true;
        }

        if (grappling || busy || grounded) return false;

        diving = true;
        busy = true;
        Vector3 flatVelocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        diveHandler = DiveHandler(flatVelocity);
        StartCoroutine(diveHandler);
        return true;
    }

    private void CheckGround()
    {
        Collider[] colliders = new Collider[1];
        Physics.OverlapSphereNonAlloc(bottom.position, 0.45f, colliders, whatIsGround);
        if (Physics.SphereCast(transform.position, 0.45f, Vector3.down, out RaycastHit hit, 1.0f, whatIsGround))
        {
            if (Vector3.Dot(Vector3.up, hit.normal) < slopeLimit)
            {
                NotGrounded();
                gravityScale = steepSlopeGravityScale;
            }
            else
            {
                if (!grounded) OnLand();
                grounded = true;
                currentGround = colliders[0].gameObject;

                SnapToGround();

                lastGroundPosition = currentGround.transform.position;
            }
            
        }
        else
        {
            NotGrounded();
        }

        lastGround = currentGround;
    }

    private void NotGrounded()
    {
        if (grounded) OnLeaveGround();
        grounded = false;
        currentGround = null;
        lastGroundPosition = Vector3.zero;
    }

    private void SnapToGround()
    {
        if (lastGround != currentGround) return;
        Vector3 delta = currentGround.transform.position - lastGroundPosition;
        transform.position += delta;
    }

    private void StepUp()
    {
        bool stepped = false;

        // Send raycast from bottom of character + stepHeight to the facing direction + the step forward offset
        Ray forwardTest = new Ray(
            bottom.position + new Vector3(0, stepHeight, 0),
            facingDirection);

        Vector2 flatVelocity = new Vector2(rb.velocity.x, rb.velocity.z);
        float adjustedForwardStepTest = 0.5f + flatVelocity.magnitude / 60.0f;

        Debug.DrawRay(bottom.position + new Vector3(0, stepHeight, 0), facingDirection);

        if (!Physics.Raycast(forwardTest, out RaycastHit _, adjustedForwardStepTest, whatIsGround))
        {
            Vector3 origin = bottom.position + new Vector3(0, stepHeight, 0) + facingDirection * adjustedForwardStepTest;
            Ray stepTest = new Ray(origin, Vector3.down);

            Debug.DrawRay(origin, Vector3.down * (stepHeight - 0.05f), Color.blue, 0.5f);

            if (Physics.Raycast(stepTest, out RaycastHit stepHit, stepHeight, whatIsGround))
            {
                // return if the ground is too steep
                if (Vector3.Dot(Vector3.up, stepHit.normal) < slopeLimit) return;

                transform.position = new Vector3(
                    transform.position.x,
                    transform.position.y + stepHeight - stepHit.distance,
                    transform.position.z);

                stepped = true;
            }
        }

        if (!stepped)
        {
            // Right test
            Vector3 cross = Vector3.Cross(facingDirection, Vector3.up);
            Vector3 rightDirection = Vector3.RotateTowards(facingDirection, -1 * cross, Mathf.Deg2Rad * 45.0f, 0.0f);
            Ray rightTest = new Ray(
                bottom.position + new Vector3(0, stepHeight, 0),
                rightDirection);

            Debug.DrawRay(bottom.position + new Vector3(0, stepHeight, 0), rightDirection);

            if (!Physics.Raycast(rightTest, out RaycastHit _, forwardStepTest, whatIsGround))
            {
                Vector3 origin = bottom.position + new Vector3(0, stepHeight, 0) + rightDirection * forwardStepTest;
                Ray stepTest = new Ray(origin, Vector3.down);

                Debug.DrawRay(origin, Vector3.down * (stepHeight - 0.05f), Color.blue, 0.5f);

                if (Physics.Raycast(stepTest, out RaycastHit stepHit, stepHeight, whatIsGround))
                {
                    // return if the ground is too steep
                    if (Vector3.Dot(Vector3.up, stepHit.normal) < slopeLimit) return;

                    transform.position = new Vector3(
                        transform.position.x,
                        transform.position.y + stepHeight - stepHit.distance,
                        transform.position.z);

                    stepped = true;
                }
            }
        }

        if (!stepped)
        {
            // Left test
            Vector3 cross = Vector3.Cross(facingDirection, Vector3.up);
            Vector3 leftDirection = Vector3.RotateTowards(facingDirection, cross, Mathf.Deg2Rad * 45.0f, 0.0f);
            Ray leftTest = new Ray(
                bottom.position + new Vector3(0, stepHeight, 0),
                leftDirection);

            Debug.DrawRay(bottom.position + new Vector3(0, stepHeight, 0), leftDirection);

            if (!Physics.Raycast(leftTest, out RaycastHit _, forwardStepTest, whatIsGround))
            {
                Vector3 origin = bottom.position + new Vector3(0, stepHeight, 0) + leftDirection * forwardStepTest;
                Ray stepTest = new Ray(origin, Vector3.down);

                Debug.DrawRay(origin, Vector3.down * (stepHeight - 0.05f), Color.blue, 0.5f);

                if (Physics.Raycast(stepTest, out RaycastHit stepHit, stepHeight, whatIsGround))
                {
                    // return if the ground is too steep
                    if (Vector3.Dot(Vector3.up, stepHit.normal) < slopeLimit) return;

                    transform.position = new Vector3(
                        transform.position.x,
                        transform.position.y + stepHeight - stepHit.distance,
                        transform.position.z);
                }
            }
        }

        // If we didn't hit anything...
        // Send another raycast from the end of the last raycast + step down offset.
        // If we hit something...
        // Snap the player's position to the top of the step
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
        while (cameraRotation.x >= 360) cameraRotation.x -= 360;
        while (cameraRotation.x < 0) cameraRotation.x += 360;
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

    // Execute buffered action
    // If we attempt an action and it fails for whatever reason,
    // store the requested action in a buffer and try to execute it next frame.
    // If the last buffered action is the same as this frame, increase the timer by 
    // delta time. If the timer is too great, then set the action to null and stop
    // trying to execute it.
    private void ExecuteBufferedAction()
    {
        if (bufferedAction != null)
        {
            if (lastBufferedAction == bufferedAction)
            {
                bufferInputTimer += Time.fixedDeltaTime;
                if (bufferInputTimer > bufferInputThres)
                {
                    bufferedAction = null;
                }
                else
                    Invoke(bufferedAction, 0.0f);
            }
            else
                bufferInputTimer = 0.0f;

            lastBufferedAction = bufferedAction;
            bufferedAction = null;
        }
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

    private void OnLeaveGround()
    {
        if (!jumping) jumpsLeft--; // If we didn't leave the ground from a jump, then take away a jump.
        if (currentGround != null)
        {
            if (currentGround.TryGetComponent(out Rigidbody groundRb))
            {
                rb.velocity += groundRb.velocity;
            }
            else
            {
                Vector3 delta = currentGround.transform.position - lastGroundPosition;
                rb.velocity += delta * 60.0f; // multiply by sixty because there are 60 physics updates in a second, converting to m/s
            }
        }
        CalculateTempAirMaxMagnitude();
    }

    private void OnLand()
    {
        jumping = false;
        dashesLeft = maxNumDashes;
        StartCoroutine(RegainJumpsAfterDelay(0.06f));
    }

    // Recalculates the temporary maximum value for air movement.
    // If shouldDecreaseMax is false, the new maximum cannot be lower than the
    // initial value.
    private void CalculateTempAirMaxMagnitude(bool shouldDecreaseMax = true)
    {
        Vector3 flatVelocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        float lastMax = tempMaxAirMag;
        tempMaxAirMag = Mathf.Max(flatVelocity.magnitude, softMaxRunSpeed);
        if (!shouldDecreaseMax)
            tempMaxAirMag = Mathf.Max(lastMax, tempMaxAirMag);
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
        jumpsLeft = maxNumJumps;
    }

    IEnumerator DashHandler(Vector3 velocity)
    {
        float timer = dashDuration;

        while (timer > 0)
        {
            yield return new WaitForFixedUpdate();
            timer -= Time.fixedDeltaTime;

            rb.velocity = velocity;
        }

        tempMaxAirMag = dashVelocityMagnitude * 0.6f;
        dashing = false;
        busy = false;
    }

    IEnumerator DiveHandler(Vector3 storedFlatVelocity)
    {
        float timer = diveMaxDuration;

        while (!grounded && timer > 0 && diving)
        {
            yield return new WaitForFixedUpdate();
            timer -= Time.fixedDeltaTime;

            rb.velocity = diveVelocity;
        }

        rb.velocity = storedFlatVelocity;
        diving = false;
        busy = false;
    }

    IEnumerator GrappleHandler(Transform grapplePoint)
    {
        if (grapplePoint == null) yield break;
        Vector3 grapplePosition = grapplePoint.position;
        // Store the velocity between frame updates in case the simulation screws with our rigidbody
        Vector3 lastVelocity = rb.velocity;
        grappleVisualScript.gameObject.SetActive(true);

        while (grappling)
        {
            yield return new WaitForFixedUpdate();

            // Update visual
            grappleVisualScript.Point1 = transform.position;
            grappleVisualScript.Point2 = grapplePosition;

            Vector3 dirToGrapplePoint = (grapplePosition - transform.position).normalized;
            if ((grapplePosition - transform.position).magnitude < grappleMinDistance) break;

            Vector3 flatDirToGrapplePoint = (
                new Vector3(grapplePosition.x, 0, grapplePosition.z)
                - new Vector3(transform.position.x, 0, transform.position.z)).normalized;
            float dot = Vector3.Dot(flatDirToGrapplePoint, desiredDirection);

            if (dot > -0.7f)
            {
                Vector3 resultingVector = Vector3.RotateTowards(dirToGrapplePoint, desiredDirection, biasToMoveDir, 0.0f);
                Vector3 newVelocity = resultingVector * lastVelocity.magnitude;
                rb.velocity = newVelocity + resultingVector * grappleAcceleration;
            }
            else
            {
                rb.velocity = dirToGrapplePoint * lastVelocity.magnitude + Vector3.up * grappleAcceleration;
            }
            lastVelocity = rb.velocity;

            // Adjust facing direction to match the velocity
            facingDirection = new Vector3(rb.velocity.x, 0, rb.velocity.z).normalized;

            // Debug
            Debug.DrawLine(transform.position, grapplePosition, Color.blue, Time.fixedDeltaTime);
        }

        grappleVisualScript.gameObject.SetActive(false);
    }

    // Input Action Callbacks
    private void OnJumpAction(InputAction.CallbackContext context)
    {
        bool jumpSuccess = Jump();
        if (!jumpSuccess)
        {
            bufferedAction = nameof(JumpBufferedAction);
        }
    }

    // Buffered action functions can't have parameters since they are invoked, so we have to make another function for the buffer
    private void JumpBufferedAction()
    {
        bool jumpSuccess = Jump();
        if (!jumpSuccess)
        {
            bufferedAction = nameof(JumpBufferedAction);
        }
    }

    private void OnDashAction(InputAction.CallbackContext context)
    {
        if (!hasDash)
            return;
        
        bool dashSuccess = Dash();
        if (!dashSuccess)
        {
            bufferedAction = nameof(DashBufferedAction);
        }
    }

    private void DashBufferedAction()
    {
        bool dashSuccess = Dash();
        if (!dashSuccess)
        {
            bufferedAction = nameof(DashBufferedAction);
        }
    }

    private void OnDiveAction(InputAction.CallbackContext context)
    {
        if (!hasDive)
            return;

        bool diveSuccess = Dive();
        if (!diveSuccess)
        {
            bufferedAction += nameof(DiveBufferedAction);
        }
    }

    private void DiveBufferedAction()
    {
        bool diveSuccess = Dive();
        if (!diveSuccess)
        {
            bufferedAction += nameof(DiveBufferedAction);
        }
    }

    private void OnInteractAction(InputAction.CallbackContext context)
    {
        Collider[] overlapped = Physics.OverlapSphere(transform.position, interactRange);

        Interactable subject = null;
        foreach (Collider collider in overlapped)
        {
            if (collider.TryGetComponent(out Interactable script))
            {
                if (subject == null || Vector3.Distance(collider.transform.position, transform.position) < Vector3.Distance(subject.transform.position, transform.position))
                {
                    subject = script;
                }
            }
        }

        if (subject != null)
            subject.Interact(Interactable.InteractMethod.Action);
    }

    private void OnGrappleStart(InputAction.CallbackContext context)
    {
        if (grappling || dashing || !hasGrapple) 
            return;

        Collider[] overlapped = Physics.OverlapSphere(transform.position, grappleRange);

        Transform grapplePoint = null;
        foreach (Collider collider in overlapped)
        {
            float camDot = Vector3.Dot(collider.transform.position - transform.position, cam.transform.forward);
            if (collider.TryGetComponent(out GrapplePoint point))
            {
                if (grapplePoint == null || camDot > Vector3.Dot(grapplePoint.position - transform.position, cam.transform.forward))
                {
                    if (diving) // Cancel dive
                    {
                        StopCoroutine(diveHandler);
                        diving = false;
                        busy = false;
                    }

                    if (busy) 
                        return;

                    grapplePoint = collider.transform;
                }
            }
        }

        if (grapplePoint != null)
        {
            grappling = true;
            grappleHandler = GrappleHandler(grapplePoint);
            StartCoroutine(grappleHandler);
        }
    }

    private void OnGrappleEnd(InputAction.CallbackContext context)
    {
        grappling = false;
    }

    public void SetHasAbility(Abilities ability, bool hasAbility)
    {
        if (ability == Abilities.DoubleJump)
        {
            hasDoubleJump = hasAbility;
            if (hasAbility)
                maxNumJumps = 2;
            else
                maxNumJumps = 1;
        }
        else if (ability == Abilities.Dash)
        {
            hasDash = hasAbility;
        }
        else if (ability == Abilities.Dive)
        {
            hasDive = hasAbility;
        }
        else if (ability == Abilities.Grapple)
        {
            hasGrapple = hasAbility;
        }
    }

    public void CollectPackage()
    {
        hasPackage = true;
        PackageCollected?.Invoke();
    }

    public void RemovePackage()
    {
        hasPackage = false;
        PackageRemoved?.Invoke();
    }

    void OnEnable()
    {
        actions.FindActionMap("gameplay").Enable();
    }
    void OnDisable()
    {
        actions.FindActionMap("gameplay").Disable();
    }

    private void OnDestroy()
    {
        // Remove Listeners
        InputActionMap actionMap = actions.FindActionMap("Gameplay");
        actionMap.FindAction("dash").performed -= OnDashAction;
        actionMap.FindAction("dive").performed -= OnDiveAction;
        actionMap.FindAction("interact").performed -= OnInteractAction;
        actionMap.FindAction("grapple").started -= OnGrappleStart;
        actionMap.FindAction("grapple").canceled -= OnGrappleEnd;
        jumpAction.performed -= OnJumpAction;
    }

    public bool IsDashing() { return dashing; }
    public bool IsDiving() { return diving; }
    public bool IsJumping() { return jumping; }
    public bool IsGrappling() { return grappling; }
    public bool HasPackage() { return hasPackage; }

    public void LoadData(GameData data)
    {
        hasPackage = data.hasPackage;
        hasDoubleJump = data.hasDoubleJump;
        hasDash = data.hasDash;
        hasDive = data.hasDive;
        hasGrapple = data.hasGrapple;
    }

    public void SaveData(ref GameData data)
    {
        data.hasPackage = hasPackage;
        data.hasDoubleJump = hasDoubleJump;
        data.hasDash = hasDash;
        data.hasDive = hasDive;
        data.hasGrapple = hasGrapple;
    }

    // Debug
    [Header("Debug")]
    public TextMeshProUGUI speedText;

    [SerializeField] private MeshRenderer mRenderer;
    [SerializeField] private Material defaultMaterial;
    [SerializeField] private Material diveMaterial;
    [SerializeField] private Material dashMaterial;
    private void DebugUpdate()
    {
        if (speedText != null)
        {
            Vector3 flatVelocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
            decimal value = Math.Round((decimal)flatVelocity.magnitude, 3);
            speedText.text = "Speed: " + value;
        }

        if (diving)
            mRenderer.material = diveMaterial;
        else if (dashing) 
            mRenderer.material = dashMaterial;
        else
            mRenderer.material = defaultMaterial;
    }
}