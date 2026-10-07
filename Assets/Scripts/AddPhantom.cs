using System;
using UnityEngine;

public class AddPhantom : MonoBehaviour
{
    [SerializeField] GameObject phantom;
    const float captureInterval = 1.5f;
    private float currentCaptureTimer;
    private LineRenderer lineRenderer;
    private bool createdPhantom = false;

    void Start()
    {
        currentCaptureTimer = captureInterval;
        lineRenderer = GetComponent<LineRenderer>();

        SetLineRenderPosition();
    }

    void Update()
    {
        currentCaptureTimer -= Time.deltaTime;
        
        if (lineRenderer.positionCount < 11 && !createdPhantom)
        {
            if (currentCaptureTimer <= 0)
            {
                currentCaptureTimer = captureInterval;
                SetLineRenderPosition();
            }           
        }
        else
        {
            if (!createdPhantom)
            {
                CreatePhantom();
                createdPhantom = true;
            }
        }
    }

    void SetLineRenderPosition()
    {
        lineRenderer.positionCount++;
        lineRenderer.SetPosition(lineRenderer.positionCount - 1, this.transform.position + new Vector3(0, .2f, 0));
    }

    void CreatePhantom()
    {
        GameObject newPhantom = Instantiate(phantom);
        LineRenderer newPhantomLineRender = newPhantom.GetComponent<LineRenderer>();

        Vector3[] tempArray = new Vector3[lineRenderer.positionCount];
        lineRenderer.GetPositions(tempArray);

        Debug.Log(string.Join(',', tempArray));

        newPhantomLineRender.positionCount = lineRenderer.positionCount;
        newPhantomLineRender.SetPositions(tempArray);

        ResetLineRenderer();
    }

    void ResetLineRenderer()
    {
        lineRenderer.positionCount = 0;
        SetLineRenderPosition();
    }
}
