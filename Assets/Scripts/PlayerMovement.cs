using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] Transform orientation;
    [SerializeField] Rigidbody rb;

    [Header("Editables")]
    // Take what you think the speed should be and multiply it by like 3.
    [SerializeField] float moveSpeed = 7f;
    [SerializeField] float groundDrag = 5f;
    // Take what you think the airspeed should be and multiply it by like 3.
    [SerializeField] float airSpeedMultiplier = .4f;
    [SerializeField] float jumpHeight = 5f;
    
    [Header("Grounded")]
    private bool isGrounded;
    [SerializeField] LayerMask groundLayer;

    // Movement Input
    private InputActionMap actionMap;
    private InputAction moveAction;
    private InputAction jumpAction;
    private Vector2 movementInput;


    void Start()
    {
        rb.freezeRotation = true;
        actionMap = InputSystem.actions.FindActionMap("Player");
        moveAction = actionMap.FindAction("Move");
        jumpAction = actionMap.FindAction("Jump");
    }

    void Update()
    {   
        ReadInput();
        CheckGrounded();
        ClampSpeed();
    }

    void FixedUpdate()
    {
        MovePlayer();
    }

    void MovePlayer()
    {
        Vector3 movementDir = orientation.forward * movementInput.y + orientation.right * movementInput.x;
        
        if (isGrounded)
        {
            rb.AddForce(movementDir.normalized * moveSpeed, ForceMode.Force);  
        }
        else if (!isGrounded)
        {
            rb.AddForce(movementDir.normalized * moveSpeed * airSpeedMultiplier, ForceMode.Force); 
        }
    }

    void ReadInput()
    {
        movementInput = moveAction.ReadValue<Vector2>();

        if (jumpAction.WasPressedThisFrame())
        {   
            Jump();
        }
    }

    void CheckGrounded()
    {
        /*
         I was having trouble with the jumping mechanic not registering that it landed, therefore I removed the "groundLayer" parameter in the
         isGrounded assignment and added a DrawRay to visually see when it can legally jump again.

            - Ryan N.
         */
        Debug.DrawRay( transform.position, Vector3.down * 1.3f, Color.red); // Added by Ryan N.
        isGrounded = Physics.Raycast(transform.position, Vector3.down, 1.3f);

        if (isGrounded)
        {
            rb.linearDamping = groundDrag;
        }
        else
        {   
            // Having drag while jumping doesn't feel good
            rb.linearDamping = 0;
        }
    }

    void ClampSpeed()
    {
        Vector3 xzVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);

        if (xzVelocity.magnitude > moveSpeed)
        {
            // Retain the direcion while reducing its magnitude to 1, then multiply by the movement speed
            Vector3 clampedVelocity = xzVelocity.normalized * moveSpeed;
            rb.linearVelocity = clampedVelocity;
        }
    }

    void Jump()
    {
        Debug.Log("isGrounded = " + isGrounded);
        if (isGrounded)
        {
            isGrounded = false;
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
            rb.AddForce(transform.up * jumpHeight, ForceMode.Impulse);
        }
    }
}
