
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAnimator : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Animator animator;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private PlayerMovement playerMovement;

    [Header("Animation Settings")]
    [SerializeField] private float movementThreshold = 0.1f;
    [SerializeField] private float animationDampTime = 0.1f;

    [Header("Prone")]
    [SerializeField] private bool toggleProne = true;

    private InputActionMap actionMap;
    private InputAction proneAction;

    private bool isProne = false;

    // Animator parameter hashes
    private readonly int speedHash = Animator.StringToHash("Speed");
    private readonly int groundedHash = Animator.StringToHash("IsGrounded");
    private readonly int verticalVelocityHash = Animator.StringToHash("VerticalVelocity");
    private readonly int proneHash = Animator.StringToHash("IsProne");

    void Start()
    {
        // Find the Player action map
        var defaultActions = InputSystem.actions;

        actionMap = defaultActions.FindActionMap("Player");

        // Find the Prone action
        proneAction = actionMap.FindAction("Prone");

        // prone isn't in movement controller, and the animation file got corrupted so shh
        if (proneAction != null)
        {
            proneAction.performed += ToggleProne;
        }
        else
        {
            Debug.LogWarning("Prone action was not found!");
        }
    }

    void Update()
    {
        UpdateMovementAnimation();
        UpdateAirAnimation();
        UpdateProneAnimation();
    }

    void UpdateMovementAnimation()
    {
        Vector3 horizontalVelocity = new Vector3(
            rb.linearVelocity.x,
            0f,
            rb.linearVelocity.z
        );

        float speed = horizontalVelocity.magnitude;

        animator.SetFloat(speedHash, speed, animationDampTime, Time.deltaTime);
    }

    void UpdateAirAnimation()
    {
        if (playerMovement == null)
            return;

        bool grounded = playerMovement.IsGrounded;

        float verticalVelocity = rb.linearVelocity.y;

        animator.SetBool(groundedHash, grounded);
        animator.SetFloat(verticalVelocityHash, verticalVelocity);
    }

    void UpdateProneAnimation()
    {
        animator.SetBool(proneHash, isProne);
    }

    void ToggleProne(InputAction.CallbackContext context)
    {
        if (!toggleProne)
            return;

        isProne = !isProne;
    }

    void OnDisable()
    {
        if (proneAction != null)
        {
            proneAction.performed -= ToggleProne;
        }
    }
}
