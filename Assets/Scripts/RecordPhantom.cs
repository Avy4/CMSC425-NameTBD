using UnityEngine;
using UnityEngine.InputSystem;

public class RecordPhantom : MonoBehaviour
{
    [SerializeField] GameObject phantom;
    [SerializeField] LineRenderer lineRenderer;
    [SerializeField] PlayerMovement playerMovement;
    const float captureInterval = 1.5f;
    float currentCaptureInterval = 0;
    private InputActionMap actionMap;
    private InputAction stopRecordingAction;
    private bool shouldCapture = true;

    void Start()
    {
        InitLine();

        actionMap = InputSystem.actions.FindActionMap("Player");
        stopRecordingAction = actionMap.FindAction("StopRecording");     

        stopRecordingAction.performed += CreatePhantom;
    }

    void Update()
    {
        currentCaptureInterval -= Time.deltaTime;

        if (currentCaptureInterval <= 0 && shouldCapture)
        {
            currentCaptureInterval = captureInterval;
            SetLineRenderPosition();
        }
    }

    void InitLine()
    {
        currentCaptureInterval = captureInterval;
        SetLineRenderPosition();
    }

    void CreatePhantom(InputAction.CallbackContext context)
    {
        shouldCapture = false;

        GameObject newPhantom = Instantiate(phantom);
        LineRenderer newPhantomLineRender = newPhantom.GetComponent<LineRenderer>();

        Vector3[] tempArray = new Vector3[lineRenderer.positionCount];
        lineRenderer.GetPositions(tempArray);

        // Debug.Log(string.Join(',', tempArray));

        newPhantomLineRender.positionCount = lineRenderer.positionCount;
        newPhantomLineRender.SetPositions(tempArray);
        
        playerMovement.ResetPlayer();
        ResetLineRenderer();
    }
    void SetLineRenderPosition()
    {
        lineRenderer.positionCount++;
        lineRenderer.SetPosition(lineRenderer.positionCount - 1, this.transform.position + new Vector3(0, .2f, 0));
    }

    void ResetLineRenderer()
    {
        lineRenderer.positionCount = 0;
        SetLineRenderPosition();
        shouldCapture = true;
    }

}
