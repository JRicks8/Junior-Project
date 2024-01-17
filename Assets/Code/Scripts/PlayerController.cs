using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private InputActionAsset actions;
    private InputAction moveAction;

    private Rigidbody rb;

    private void Awake()
    {
        actions.FindActionMap("Gameplay").FindAction("jump").performed += OnJumpAction;

        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
    }

    private void Update()
    {
    }

    private void OnJumpAction(InputAction.CallbackContext context)
    {

    }

    private void OnCrouchAction(InputAction.CallbackContext context)
    {

    }

    private void OnInteractAction(InputAction.CallbackContext context)
    {

    }
}
