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
    public float phantomSpeed = 3.5f;

    #region Phantom Vars
    private LineRenderer phantomLineRenderer;
    private Rigidbody phantomBody;
    #endregion

    #region Phantom Follow Vars
    private Vector3[] phantomPositions;
    private int currentPositionIdx;
    private Vector3 currentDestination;
    #endregion
    
    void Start()
    {
        // Get Components
        phantomLineRenderer = GetComponent<LineRenderer>();
        phantomBody = GetComponent<Rigidbody>();

        // Initialize line renderer with phantom positions in RecordPhantom
        phantomPositions = new Vector3[phantomLineRenderer.positionCount];
        phantomLineRenderer.GetPositions(phantomPositions);
        RestartPath();
    }

    void FixedUpdate()
    {
        // Interpolate new position from current position and destination
        Vector3 newPos = Vector3.MoveTowards(phantomBody.transform.position, currentDestination, phantomSpeed * Time.deltaTime);

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
        float distance = (phantomBody.transform.position - currentDestination).magnitude;

        // Checks if magnitude of position - destination is very small 
        if (distance < .05f)
        {
            return true;
        }

        return false;
    }

    // Restart is ONLY called when Q is hit or the player dies
    void RestartPath()
    {   
        // Idx, position restart
        currentPositionIdx = 0;
        phantomBody.transform.position = phantomPositions[currentPositionIdx];

        // Idx iterate, set new destination
        currentPositionIdx++;
        currentDestination = phantomPositions[currentPositionIdx];
    }
}   
