using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class CapstandTurn : MonoBehaviour
{


    //serializeFeild to make these private variables changable and visible in the Inspector
    [SerializeField] private Transform rotatingAssembly; //to be assigned by us
    [SerializeField] private Transform door; //to be assigned by us
    [SerializeField, Min(0f)] private float rotationSpeed = 45f;
    [SerializeField, Min(1f)] private float rotationForFullOpen = 360f;
    [SerializeField, Min(0f)] private float doorOpenHeight = 7.5f;
    [SerializeField, Min(0.01f)] private float doorSmoothTime = 0.5f;
    [SerializeField, Min(0.01f)] private float idleCloseDelay = 2f;

    private Vector3 doorClosedPosition;
    private Vector3 doorVelocity;
    private float rotationProgress;
    private float lastInputTime;
    private bool hasReceivedInput;
    private bool isReturning;
    private readonly Dictionary<int, int> playerRotationFrames = new Dictionary<int, int>();

    void Awake()
    {
        
        if (GetComponent<Rigidbody>() == null)
        {
            Rigidbody triggerBody = gameObject.AddComponent<Rigidbody>();
            triggerBody.isKinematic = true;
            triggerBody.useGravity = false;
        }

        if (rotatingAssembly == null)
        {
            Debug.LogError("Assign the capstan's shared parent pivot to Rotating Assembly in the Inspector.", this);
            enabled = false;
            return;
        }

        if (door != null)
        {
            doorClosedPosition = door.position;
        }
        else
        {
            Debug.LogWarning("No door is assigned; the capstan will rotate without moving a door.", this);
        }
    }

    void Update()
    {
        if (hasReceivedInput && rotationProgress > 0f &&
            Time.time - lastInputTime >= idleCloseDelay)
        {
            isReturning = true;
        }

        if (isReturning && rotationProgress > 0f)
        {
            float reverseRotation = Mathf.Min(
                rotationSpeed * Time.deltaTime, rotationProgress);
            rotatingAssembly.Rotate(Vector3.up, -reverseRotation, Space.World);
            rotationProgress -= reverseRotation;
        }

        if (door != null)
        {
            float openAmount = Mathf.Clamp01(rotationProgress / rotationForFullOpen);
            Vector3 doorTargetPosition = doorClosedPosition + Vector3.up * (doorOpenHeight * openAmount);
            door.position = Vector3.SmoothDamp(
                door.position, doorTargetPosition, ref doorVelocity, doorSmoothTime);
        }
    }

    void OnTriggerStay(Collider other)
    {
        StarterAssets.StarterAssetsInputs playerInput =
            other.GetComponentInParent<StarterAssets.StarterAssetsInputs>();

        if (playerInput == null)
        {
            return;
        }

        float inputAmount = Mathf.Max(
            Mathf.Abs(playerInput.move.x), Mathf.Abs(playerInput.move.y));
        if (inputAmount <= 0f)
        {
            return;
        }

        int playerId = playerInput.gameObject.GetInstanceID();
        if (playerRotationFrames.TryGetValue(playerId, out int lastFrame) &&
            lastFrame == Time.frameCount)
        {
            return;
        }

        playerRotationFrames[playerId] = Time.frameCount;
        lastInputTime = Time.time;
        hasReceivedInput = true;
        isReturning = false;

        float rotation = Mathf.Min(
            rotationSpeed * inputAmount * Time.deltaTime,
            rotationForFullOpen - rotationProgress);

        if (rotation <= 0f)
        {
            return;
        }

        rotatingAssembly.Rotate(Vector3.up, rotation, Space.World);
        rotationProgress += rotation;
    }
}
