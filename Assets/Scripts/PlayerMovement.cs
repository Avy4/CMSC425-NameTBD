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
    private float radius = .4f;
    private float maxDistance = .2f;
    private const float skinWidth = 0.05f;
    private readonly RaycastHit[] groundHits = new RaycastHit[8];
    private Vector3 anchorModifier = new Vector3(0, .5f, 0);  // ? what this for?
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
        Collider feetCollider = null;
        float feetY = float.PositiveInfinity;

        foreach (Collider col in rb.GetComponentsInChildren<Collider>(false))
        {
            if (!col.enabled || col.isTrigger) continue;                    // only solid colliders
            if (col.attachedRigidbody != null && col.attachedRigidbody != rb) continue; // skip other bodies

            if (col.bounds.min.y < feetY)
            {
                feetY = col.bounds.min.y;
                feetCollider = col;
            }
        }
        
        Vector3 origin = new Vector3(
            feetCollider.bounds.center.x,
            feetY + skinWidth + radius,
            feetCollider.bounds.center.z);
        float castDistance = skinWidth + maxDistance;

        int hitCount = Physics.SphereCastNonAlloc(
            origin, radius, Vector3.down, groundHits, castDistance,
            groundLayer.value, QueryTriggerInteraction.Ignore);

        isGrounded = false;
        for (int i = 0; i < hitCount; i++)
        {
            Collider hitCollider = groundHits[i].collider;

            if (hitCollider == null) continue;
            if (hitCollider.transform.IsChildOf(transform)) continue;   // never ground on ourselves
            if (groundHits[i].normal.y < 0.5f) continue;                // ignore walls, only walkable slopes

            isGrounded = true;
            break;
        }
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
    public bool IsGrounded => isGrounded;
    #endregion

    public void ResetPlayer()
    {
        movementInput = Vector2.zero;
        rb.linearVelocity = Vector3.zero;
        isGrounded = true;

        rb.transform.position = initialPosition;
    }
}

