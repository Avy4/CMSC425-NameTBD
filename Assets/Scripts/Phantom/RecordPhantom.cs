using UnityEngine;
using UnityEngine.InputSystem;

public class RecordPhantom : MonoBehaviour
{
    [SerializeField] GameObject phantom;
    // Do NOT confuse this LineRenderer with the one on the phantom
    // This LineRenderer copies the path of the player, the current path
    // The LineRenderer on the phantom contains the path that the phantom copied
    [SerializeField] LineRenderer lineRenderer;
    [SerializeField] PlayerDeath playerDeath;

    #region Capture Variables
    private const float captureInterval = .05f;
    private float currentCaptureInterval = 0;
    private bool shouldCapture = true;
    // Keeps the phantom body from going inside the ground
    private const float yPosAgument = .2f;
    #endregion

    #region Actions
    private InputActionMap actionMap;
    private InputAction stopRecordingAction;
    #endregion

    void Start()
    {   
        InitLine();

        // Setup Actions
        actionMap = InputSystem.actions.FindActionMap("Player");
        stopRecordingAction = actionMap.FindAction("StopRecording");     

        // This is adding a callback method
        stopRecordingAction.performed += CreatePhantomCallback;
    }

    void Update()
    {
        currentCaptureInterval -= Time.deltaTime;

        // Checks if captureInterval has elapsed and captures (if it is allowed to)
        if (currentCaptureInterval <= 0 && shouldCapture)
        {
            currentCaptureInterval = captureInterval;
            SetLineRenderPosition();
        }
    }

    // Init vars, could put in start but this is cleaner
    void InitLine()
    {   
        currentCaptureInterval = captureInterval;
        SetLineRenderPosition();
    }
    void CreatePhantomCallback(InputAction.CallbackContext context)
    {   
        // Resetting with Q and Dying have the same functionality
        // Less reuse of code, and removes uneeded connections
        playerDeath.Die();
    }
    public void CreatePhantom()
    {
        shouldCapture = false;

        // Instantiates a new Phantom
        GameObject newPhantom = Instantiate(phantom);
        LineRenderer newPhantomLineRender = newPhantom.GetComponent<LineRenderer>();

        // Sets its LineRenderer to match the currently copied path
        Vector3[] tempArray = new Vector3[lineRenderer.positionCount];
        lineRenderer.GetPositions(tempArray);

        newPhantomLineRender.positionCount = lineRenderer.positionCount;
        newPhantomLineRender.SetPositions(tempArray);
        
        // Fresh Line Renderer
        ResetLineRenderer();
    }

    // Creates a new captured position in the LineRenderer
    void SetLineRenderPosition()
    {
        lineRenderer.positionCount++;
        lineRenderer.SetPosition(lineRenderer.positionCount - 1, this.transform.position + new Vector3(0, yPosAgument, 0));
    }

    // Resets all the points in the line renderer
    void ResetLineRenderer()
    {
        lineRenderer.positionCount = 0;
        SetLineRenderPosition();
        shouldCapture = true;
    }
}
