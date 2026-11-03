using UnityEngine;

public class ButtonPress : MonoBehaviour
{
    [SerializeField] private Transform door;
    [SerializeField, Min(0.01f)] private float doorSmoothTime = 0.5f;

    private Vector3 startingPosition;
    private Vector3 doorStartingPosition;
    private Vector3 doorTargetPosition;
    private Vector3 doorVelocity;
    private bool isPressed;

    void Awake()
    {
        startingPosition = transform.localPosition;

        if (door != null)
        {
            doorStartingPosition = door.position;
            doorTargetPosition = doorStartingPosition;
        }
    }

    void Update()
    {
        if (door != null)
        {
            door.position = Vector3.SmoothDamp(
                door.position, doorTargetPosition, ref doorVelocity, doorSmoothTime);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log($"Button triggered by {other.name}", this);

        if (!isPressed)
        {
            transform.localPosition = startingPosition + Vector3.down * 0.25f;

            if (door == null)
            {
                Debug.LogError("Assign the door Transform in the Inspector.", this);
            }
            else
            {
                doorTargetPosition = doorStartingPosition + Vector3.up * 4;
            }

            isPressed = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (isPressed)
        {
            transform.localPosition = startingPosition;

            if (door != null)
            {
                doorTargetPosition = doorStartingPosition;
            }

            isPressed = false;
        }
    }
}
