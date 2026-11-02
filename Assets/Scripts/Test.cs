using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.InputSystem;

public class Test : MonoBehaviour
{
    private InputActionMap actionMap;
    private InputAction moveAction;
    private InputAction jumpAction;

    [SerializeField] Transform orientation;
    [SerializeField] Rigidbody rb;
    // Jump Check
    [SerializeField] LayerMask groundLayer;
    bool isGrounded = true;
    float radius = .5f;
    float maxDistance = 0f;

    Vector2 movementInput;
    bool isJumping = false;

    [SerializeField] float gravity = -9.81f;

    void Update()
    {
        ReadInput();
    }

    void ReadInput()
    {
        movementInput = moveAction.ReadValue<Vector2>();
        
        if (isGrounded && jumpAction.ReadValue<bool>())
        {
        }
    }

    void GroundCheck()
    {
        isGrounded = Physics.SphereCast(transform.position - new Vector3(0, 1, 0), radius, Vector3.down, out _, maxDistance, groundLayer.value);
    }

    void MovePlayer()
    {
        Vector3 movementDir = orientation.forward * movementInput.y + orientation.right * movementInput.x;
    }

}
