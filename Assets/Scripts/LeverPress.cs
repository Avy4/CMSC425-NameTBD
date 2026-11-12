using UnityEngine;

public class LeverPress : MonoBehaviour
{
    [SerializeField] private Transform leverArm;
    [SerializeField] private Transform door;
    [SerializeField, Min(0.01f)] private float doorSmoothTime = 0.5f;

    private bool isRotated;
    private Vector3 leverArmStartingPosition;
    private Vector3 doorStartingPosition;
    private Vector3 doorTargetPosition;
    private Vector3 doorVelocity;
    private bool isDoorOpen;

    void Awake()
    {
        if (leverArm == null)
        {
            foreach (Transform child in GetComponentsInChildren<Transform>(true))
            {
                if (child != transform && child.name == "LeverArm")
                {
                    leverArm = child;
                    break;
                }
            }
        }

        if (leverArm != null)
        {
            leverArmStartingPosition = leverArm.localPosition;
        }

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
        Debug.Log($"Lever triggered by {other.name}", this);

        if (leverArm == null)
        {
            Debug.LogError("Assign the lever arm Transform in the Inspector or name a child object 'LeverArm'.", this);
            return;
        }

        float rotation = isRotated ? -25f : 25f;
        leverArm.Rotate(rotation, 0f, 0f, Space.Self);
        leverArm.localPosition = leverArmStartingPosition +
            (isRotated ? Vector3.zero : Vector3.left * 0.25f);
        isRotated = !isRotated;

        if (door == null)
        {
            Debug.LogError("Assign the door Transform in the Inspector.", this);
            return;
        }

        isDoorOpen = !isDoorOpen;
        doorTargetPosition = isDoorOpen
            ? doorStartingPosition + Vector3.up * 7.5f
            : doorStartingPosition;
    }
}
