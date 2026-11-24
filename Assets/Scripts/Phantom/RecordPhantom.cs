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
    [SerializeField] int maxPhantoms = 1;

    #region Capture Variables
    [SerializeField] float minimumCaptureTime = 3f;
    private PhantomManager phantomManager;
    // ~Every 3 fixed update cycles
    private const float captureInterval = 0.06f;
    private float currentCaptureInterval = 0;
    private bool shouldCapture = true;
    // Keeps the phantom body from going inside the ground
    private const float yPosAugment = .2f;
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

        // Create Phantom Manager
        phantomManager = new PhantomManager(maxPhantoms);

        // This is adding a callback method
        stopRecordingAction.performed += CreatePhantomCallback;
    }

    void OnDisable()
    {
        // Unsubscribes callback when GameObject is destroyed
        stopRecordingAction.performed -= CreatePhantomCallback;
    }

    void FixedUpdate()
    {
        if (shouldCapture)
        {
            currentCaptureInterval -= Time.fixedDeltaTime;

            // Checks if captureInterval has elapsed and captures (if it is allowed to)
            if (currentCaptureInterval <= 0)
            {
                currentCaptureInterval = captureInterval;
                SetLineRenderPosition();
            }
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
        if (lineRenderer.positionCount > (minimumCaptureTime / captureInterval))
        {
            playerDeath.Die();
        }
    }
    public void CreatePhantom()
    {
        shouldCapture = false;

        // Capture an extra position just in case
        if (currentCaptureInterval <= captureInterval / 2)
        {
            SetLineRenderPosition();
        }

        // Instantiates a new Phantom
        GameObject newPhantom = Instantiate(phantom);
        LineRenderer newPhantomLineRender = newPhantom.GetComponent<LineRenderer>();

        // Sets its LineRenderer to match the currently copied path
        Vector3[] tempArray = new Vector3[lineRenderer.positionCount];
        lineRenderer.GetPositions(tempArray);

        newPhantomLineRender.positionCount = lineRenderer.positionCount;
        newPhantomLineRender.SetPositions(tempArray);

        phantomManager.AddPhantom(newPhantom);
        phantomManager.ResetPhantoms();

    }

    // Creates a new captured position in the LineRenderer
    void SetLineRenderPosition()
    {
        lineRenderer.positionCount++;
        lineRenderer.SetPosition(lineRenderer.positionCount - 1, this.transform.position + new Vector3(0, yPosAugment, 0));
    }

    // Resets all the points in the line renderer
    public void ResetLineRenderer()
    {
        lineRenderer.positionCount = 0;
        SetLineRenderPosition();
        shouldCapture = true;

        // Reset the captureInterval so we don't get a quick capture in the next iteration
        currentCaptureInterval = captureInterval;
    }
}
