using Unity.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class ThirdPersonCamera : MonoBehaviour
{
    // Dependendencies
    [SerializeField] Transform orientation;
    [SerializeField] Transform player;
    [SerializeField] Transform playerModel;
    [SerializeField] float rotationSpeed = 7f;

    InputActionMap actionMap;
    InputAction moveAction;

    private void Start()
    {
        actionMap = InputSystem.actions.FindActionMap("Player");
        moveAction = actionMap.FindAction("Move");
    }

    private void Update()
    {   
        // "this" refers to this Cameras position
        Vector3 cameraDir = player.position - new Vector3(this.transform.position.x, player.position.y, this.transform.position.z);
        orientation.forward = cameraDir.normalized;

        RotateCam();
    }

    private void RotateCam()
    {   
        Vector2 movementInput = moveAction.ReadValue<Vector2>();
        Debug.Log(movementInput);
        Vector3 movementDir = orientation.forward * movementInput.y + orientation.right * movementInput.x;

        if (movementDir != Vector3.zero)
        {
            playerModel.forward = Vector3.Slerp(playerModel.forward, movementDir.normalized, Time.deltaTime * rotationSpeed);
        }
    }
}
