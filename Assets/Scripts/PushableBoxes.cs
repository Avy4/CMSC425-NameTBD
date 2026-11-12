using UnityEngine;
using UnityEngine.InputSystem;

public class PushableBoxes : MonoBehaviour
{
    private InputActionMap actionMap;
    private InputAction stopRecordingAction;
    [SerializeField] Rigidbody rb;
    [SerializeField] Vector3 startPosition;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        actionMap = InputSystem.actions.FindActionMap("Player");
        stopRecordingAction = actionMap.FindAction("StopRecording");   
        startPosition = transform.position;
        rb.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotation;
    }

    // Update is called once per frame
    void Update()
    {
        // TODO: should probably add something that makes it also reset upon the player dying, not just when the player manually resets.
        if (stopRecordingAction.WasPressedThisFrame())
        {
            rb.transform.position = startPosition;
            rb.linearVelocity = Vector3.zero;
        }
    }
}
