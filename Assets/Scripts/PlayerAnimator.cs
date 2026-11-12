
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
        // Self-heal lost references so the controller never silently stops working.
        if (animator == null) animator = GetComponentInChildren<Animator>();
        if (rb == null) rb = GetComponentInParent<Rigidbody>();
        if (playerMovement == null) playerMovement = GetComponentInParent<PlayerMovement>();

        if (animator == null)
            Debug.LogError("PlayerAnimator: no Animator assigned/found. Animations will not play.");
        if (rb == null)
            Debug.LogError("PlayerAnimator: no Rigidbody assigned/found. Movement animation will not update.");
        if (playerMovement == null)
            Debug.LogError("PlayerAnimator: no PlayerMovement assigned/found. Grounded state will not update.");

        // Find the Player action map
        var defaultActions = InputSystem.actions;
        if (defaultActions == null)
        {
            Debug.LogWarning("PlayerAnimator: no default InputActions asset is set (Project Settings > Input System). Prone input disabled.");
            return;
        }

        actionMap = defaultActions.FindActionMap("Player");
        if (actionMap == null)
        {
            Debug.LogWarning("PlayerAnimator: 'Player' action map was not found. Prone input disabled.");
            return;
        }

        // Find the Prone action
        proneAction = actionMap.FindAction("Prone");

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
        if (animator == null || rb == null)
            return; // references missing - already reported in Start, avoid NRE spam

        UpdateMovementAnimation();
        UpdateAirAnimation();
        UpdateProneAnimation();
    }

    void UpdateMovementAnimation()
    {
        // Get horizontal velocity (ignore vertical movement)
        Vector3 horizontalVelocity = new Vector3(
            rb.linearVelocity.x,
            0f,
            rb.linearVelocity.z
        );

        float speed = horizontalVelocity.magnitude;

        // Update walking animation speed
        animator.SetFloat(speedHash, speed, animationDampTime, Time.deltaTime);
    }

    void UpdateAirAnimation()
    {
        if (playerMovement == null)
            return;

        // Grounded state from PlayerMovement
        bool grounded = playerMovement.IsGrounded;

        // Current vertical velocity
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
