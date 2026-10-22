using UnityEngine;

public class Phantom : MonoBehaviour
{
    private LineRenderer phantomLineRenderer;
    private Rigidbody phantomBody;
    private Vector3[] phantomPositions;
    private Vector3 currentDestination;
    private int currentIdx;
    const float captureInterval = 1.5f;
    private float currentCaptureTimer;

    void Start()
    {
        phantomLineRenderer = GetComponent<LineRenderer>();
        phantomBody = GetComponent<Rigidbody>();

        phantomPositions = new Vector3[phantomLineRenderer.positionCount];
        phantomLineRenderer.GetPositions(phantomPositions);
        RestartPath();
    }

    void FixedUpdate()
    {
        Vector3 newPos = Vector3.MoveTowards(phantomBody.transform.position, currentDestination, 3.5f * Time.deltaTime);

        phantomBody.MovePosition(newPos);

        if (ClosenessCheck() && TimeCheck())
        {
            if (currentIdx + 1 < phantomPositions.Length)
            {
                currentDestination = phantomPositions[++currentIdx];
            }
            else
            {
                RestartPath();
            }
        }
    }

    bool ClosenessCheck()
    {
        float distance = (phantomBody.transform.position - currentDestination).magnitude;

        if (distance < .05)
        {
            return true;
        }

        return false;
    }

    bool TimeCheck()
    {
        currentCaptureTimer -= Time.deltaTime;
        if (currentCaptureTimer <= 0)
        {
            currentCaptureTimer = captureInterval;
            return true;
        }

        return false;
    }

    void RestartPath()
    {
        currentIdx = 0;
        phantomBody.transform.position = phantomPositions[currentIdx++];
        currentDestination = phantomPositions[currentIdx];

        currentCaptureTimer = captureInterval;
    }
}   
