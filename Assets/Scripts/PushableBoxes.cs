using System;
using UnityEngine;

public class PushableBoxes : MonoBehaviour
{
    // Don't want to keep a running list of every PushableBox so we just use an action to reset them all
    public static event Action OnReset;
    [SerializeField] Rigidbody rb;
    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position;
        rb.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotation;
        OnReset += ResetBlock; 
    }
    
    void OnDisable()
    {
        OnReset -= ResetBlock;
    }

    // Static function that resets all blocks
    public static void AlertResetBlock()
    {
        if (OnReset != null)
        {
            OnReset.Invoke();
        }
    }

    void ResetBlock()
    {
        rb.position = startPosition;
    }
}
