using UnityEngine;
using UnityEngine.InputSystem;
public class ThirdPersonCamera : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] Transform orientation;
    [SerializeField] Transform player;
    [SerializeField] Transform playerModel;

    [Header("Editables")]
    [SerializeField] float rotationSpeed = 7f;

    // Movement Input
    private InputActionMap actionMap;
    private InputAction moveAction;

    private void Start()
    {
        actionMap = InputSystem.actions.FindActionMap("Player");
        moveAction = actionMap.FindAction("Move");
    }

    private void Update()
    {   
        // "this" refers to this Cameras position
        Vector3 cameraDir = player.position - new Vector3(this.transform.position.x, player.position.y, this.transform.position.z);

        // Changes the forward direction to the direction that the camera is facing.
        // Its based of this direction that you move left or right. 
        orientation.forward = cameraDir.normalized;

        RotateCam();
    }

    private void RotateCam()
    {   
        // Reads in value, the direction of movement is your input in the y (which is the z) and the x added together
        // The direction of movement is relative to the orientation we are in
        Vector2 movementInput = moveAction.ReadValue<Vector2>();
        Vector3 movementDir = orientation.forward * movementInput.y + orientation.right * movementInput.x;

        if (movementDir != Vector3.zero)
        {   
            // Rotate the model to match the direction of movement
            playerModel.forward = Vector3.Slerp(playerModel.forward, movementDir.normalized, Time.deltaTime * rotationSpeed);
        }
    }
}
