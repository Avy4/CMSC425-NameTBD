using System.Collections;
using UnityEngine;

/*
    Phantom Functionality:
    - Phantom is continous, follows its path without stopping until it reaches its endpoint
    - If the phantom dies, it remains dead until the player restarts
    - The phantom restarts if the player restarts (presses q or dies)
*/

public class PhantomMovement : MonoBehaviour
{
    // Editor Vars
    public float phantomSpeed = 6f;

    #region Phantom Vars
    private LineRenderer phantomLineRenderer;
    private Rigidbody phantomBody;
    private CapsuleCollider phantomCollider;
    private float disableTime = 1f;
    #endregion

    #region Phantom Follow Vars
    private Vector3[] phantomPositions;
    private int currentPositionIdx;
    private Vector3 currentDestination;
    #endregion

    private bool isSetup = false;

    void Awake()
    {
        // Get Components
        phantomLineRenderer = GetComponent<LineRenderer>();
        phantomBody = GetComponent<Rigidbody>();
        phantomCollider = GetComponentInChildren<CapsuleCollider>();

        // Removes the jitteriness from the RigidBody
        phantomBody.interpolation = RigidbodyInterpolation.Interpolate;
    }

    void FixedUpdate()
    {
        // Interpolate new position from current position and destination
        Vector3 newPos = Vector3.MoveTowards(phantomBody.position, currentDestination, phantomSpeed * Time.fixedDeltaTime);

        // Moves
        phantomBody.MovePosition(newPos);

        // ClosenessCheck ensures you stay on the path
        if (ClosenessCheck())
        {
            // Bounds Check
            if (currentPositionIdx + 1 < phantomPositions.Length)
            {   
                // Iterate
                currentPositionIdx += 1;
                currentDestination = phantomPositions[currentPositionIdx];
            }
        }
    }

    bool ClosenessCheck()
    {   
        // Gets magintude (we don't care about direction)
        float distance = (phantomBody.position - currentDestination).magnitude;

        // Checks if magnitude of position - destination is very small 
        if (distance < .05f)
        {
            return true;
        }

        return false;
    }

    // Restart is ONLY called when Q is hit or the player dies
    public void RestartPath()
    {   
        // Called by PhantomManager to reset the phantom path
        // Before the first reset the phantom positions have to be initialized so we put this here
        // This setup is in RestartPath() instead of Awake/Start because this makes sure it's in the right order regardless of Unity execution
        if (!isSetup)
        {   
            // Initialize line renderer with phantom positions in RecordPhantom
            phantomPositions = new Vector3[phantomLineRenderer.positionCount];

            phantomLineRenderer.GetPositions(phantomPositions);

            isSetup = true;
        }

        StartCoroutine(NoCollision());

        // Idx, position restart
        currentPositionIdx = 0;

        phantomBody.position = phantomPositions[currentPositionIdx];

        // Idx iterate, set new destination
        currentPositionIdx++;
        currentDestination = phantomPositions[currentPositionIdx]; 
    }

    // When all the GameObjects return to the start position at the same time they collide and can shoot up
    // This essentially makes it so this never happens
    // As a coroutine because it wouldn't make sense in update (using a bool) since we want it to happen so often
    private IEnumerator NoCollision()
    {
        if (phantomCollider != null)
        {
            phantomCollider.enabled = false;
        }

        yield return new WaitForSeconds(disableTime);

        phantomCollider.enabled = true;
    }
}   
