using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] Transform orientation;
    [SerializeField] Rigidbody rb;

    #region Physics Variables
    public float moveSpeed = 7f;
    public float jumpForce = 6f;
    #endregion

    // Initial Position that the player resets to when hitting Q or dying
    [SerializeField] Vector3 initialPosition;

    #region Actions
    private InputActionMap actionMap;
    private InputAction moveAction;
    private InputAction jumpAction;
    private Vector2 movementInput = Vector2.zero;
    private bool triedToJump = false;
    #endregion
    
    #region GroundCheck
    [SerializeField] LayerMask groundLayer;
    [SerializeField] LayerMask phantomLayer;
    private float radius = .4f;
    private float maxDistance = .2f;
    private Vector3 anchorModifier = new Vector3(0, .5f, 0); 
    private bool isGrounded = true; 
    #endregion

    void Start()
    {
        // Removes the jitteriness from the RigidBody
        rb.interpolation = RigidbodyInterpolation.Interpolate;

        // Freeze rotation so the player doesnt fall over
        rb.constraints = RigidbodyConstraints.FreezeRotation; 

        actionMap = InputSystem.actions.FindActionMap("Player");
        moveAction = actionMap.FindAction("Move");
        jumpAction = actionMap.FindAction("Jump");

        moveAction.performed += ReadMovementInput;
        moveAction.canceled += ResetMovementInput;

        jumpAction.performed += ReadJumpInput;
        jumpAction.canceled += ResetJumpInput;       
    }

    void FixedUpdate()
    {
        GroundCheck();
        Jump();
        Move();
    }

    void GroundCheck()
    {
        // Raycasts a Sphere down from position - anchorModifier. If it hits the groundLayer or the phantomLayer you are grounded.
        isGrounded = Physics.SphereCast(transform.position - anchorModifier, radius, Vector3.down, out _, maxDistance, (1 << groundLayer) | (1 << phantomLayer));
    }
    
    void Move()
    {   
        // Direction
        Vector3 movementDir = (orientation.forward * movementInput.y + orientation.right * movementInput.x).normalized;

        // Direction * moveSpeed (keeps the current y velocity)
        Vector3 velocity = new Vector3(movementDir.x * moveSpeed, rb.linearVelocity.y, movementDir.z * moveSpeed);

        // Sets velocity
        rb.linearVelocity = velocity;
    }

    void Jump()
    {   
        if (triedToJump && isGrounded)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
            rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
        }
    }

    #region Input 
    void ReadMovementInput(InputAction.CallbackContext context)
    {
        movementInput = moveAction.ReadValue<Vector2>();
    }

    void ResetMovementInput(InputAction.CallbackContext context)
    {
        movementInput = Vector2.zero;
    }

    void ReadJumpInput(InputAction.CallbackContext context)
    {
        triedToJump = true;
    }

    void ResetJumpInput(InputAction.CallbackContext context)
    {
        triedToJump = false;

    }
    #endregion

    public void ResetPlayer()
    {
        movementInput = Vector2.zero;
        rb.linearVelocity = Vector3.zero;
        isGrounded = true;

        // Position Reset
        rb.transform.position = initialPosition;
    }
}

