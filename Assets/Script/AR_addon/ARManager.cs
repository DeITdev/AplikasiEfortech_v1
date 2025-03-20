using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class ARManager : MonoBehaviour
{
    [Header("AR Components")]
    private CustomARPlacement customPlacement;
    private ARPlaneManager planeManager;
    private ARSession arSession;

    [Header("UI Control")]
    [SerializeField] private GameObject deleteButton;

    [Header("Graph Control")]
    [SerializeField] private GraphState graphState;
    [SerializeField] private GameObject graphButton;
    [SerializeField] private GameObject noGraphButton;

    [Header("Lock Control")]
    [SerializeField] private LockPositionState lockPositionState;
    [SerializeField] private GameObject lockButton;
    [SerializeField] private GameObject unlockButton;

    private bool isObjectSpawned = false;

    private void Awake()
    {
        customPlacement = FindObjectOfType<CustomARPlacement>();
        planeManager = FindObjectOfType<ARPlaneManager>();
        arSession = FindObjectOfType<ARSession>();

        // Initially hide all buttons
        if (deleteButton != null) deleteButton.SetActive(false);
        if (lockButton != null) lockButton.SetActive(false);
        if (unlockButton != null) unlockButton.SetActive(false);
        if (graphButton != null) graphButton.SetActive(false);
        if (noGraphButton != null) noGraphButton.SetActive(false);

        // Subscribe to spawn events
        CustomARPlacement.OnObjectSpawned += HandleObjectSpawned;
        CustomARPlacement.OnObjectDestroyed += HandleObjectDestroyed;

        // Initialize state subscriptions
        if (graphState != null)
        {
            graphState.OnGraphStateChanged += UpdateGraphButtonVisibility;
        }
        if (lockPositionState != null)
        {
            lockPositionState.OnLockStateChanged += UpdateLockButtonVisibility;
        }
    }

    private void OnDestroy()
    {
        CustomARPlacement.OnObjectSpawned -= HandleObjectSpawned;
        CustomARPlacement.OnObjectDestroyed -= HandleObjectDestroyed;

        if (graphState != null)
        {
            graphState.OnGraphStateChanged -= UpdateGraphButtonVisibility;
        }
        if (lockPositionState != null)
        {
            lockPositionState.OnLockStateChanged -= UpdateLockButtonVisibility;
        }
    }

    private void HandleObjectSpawned()
    {
        isObjectSpawned = true;

        // Update button visibility based on current states
        if (graphState != null)
        {
            UpdateGraphButtonVisibility(graphState.IsGraphVisible);
        }
        if (lockPositionState != null)
        {
            UpdateLockButtonVisibility(lockPositionState.IsUnlocked);
        }
    }

    private void HandleObjectDestroyed()
    {
        isObjectSpawned = false;
        if (deleteButton != null)
        {
            deleteButton.SetActive(false);
        }
        // Hide all buttons
        if (graphButton != null) graphButton.SetActive(false);
        if (noGraphButton != null) noGraphButton.SetActive(false);
        if (lockButton != null) lockButton.SetActive(false);
        if (unlockButton != null) unlockButton.SetActive(false);
    }

    private void UpdateGraphButtonVisibility(bool isGraphVisible)
    {
        if (isObjectSpawned)
        {
            if (graphButton != null) graphButton.SetActive(isGraphVisible);
            if (noGraphButton != null) noGraphButton.SetActive(!isGraphVisible);
        }
        else
        {
            if (graphButton != null) graphButton.SetActive(false);
            if (noGraphButton != null) noGraphButton.SetActive(false);
        }
    }

    private void UpdateLockButtonVisibility(bool isUnlocked)
    {
        if (isObjectSpawned)
        {
            // When isUnlocked = true (free to move):
            // - Show unlock button (clicking it will lock the object)
            // - Hide lock button
            // When isUnlocked = false (locked):
            // - Show lock button (clicking it will unlock the object)
            // - Hide unlock button
            if (unlockButton != null) unlockButton.SetActive(isUnlocked);
            if (lockButton != null) lockButton.SetActive(!isUnlocked);

            // Show delete button only when object is freely moving (unlocked)
            if (deleteButton != null)
            {
                deleteButton.SetActive(isUnlocked);
            }
        }
        else
        {
            // Hide all buttons when no object is spawned
            if (unlockButton != null) unlockButton.SetActive(false);
            if (lockButton != null) lockButton.SetActive(false);
            if (deleteButton != null) deleteButton.SetActive(false);
        }
    }

    public void OnGraphButtonClick()
    {
        if (graphState != null)
        {
            graphState.IsGraphVisible = false;
        }
    }

    public void OnNoGraphButtonClick()
    {
        if (graphState != null)
        {
            graphState.IsGraphVisible = true;
        }
    }

    public void OnUnlockButtonClick()
    {
        if (lockPositionState != null && customPlacement != null)
        {
            // User clicked unlock button (which means object is free to move)
            // So we want to lock it
            lockPositionState.IsUnlocked = false;
            customPlacement.SetLockState(true);

            // Disable plane detection
            if (planeManager != null)
            {
                planeManager.enabled = false;
                foreach (var plane in planeManager.trackables)
                {
                    plane.gameObject.SetActive(false);
                }
            }
        }
    }

    public void OnLockButtonClick()
    {
        if (lockPositionState != null && customPlacement != null)
        {
            // User clicked lock button (which means object is locked)
            // So we want to unlock it
            lockPositionState.IsUnlocked = true;
            customPlacement.SetLockState(false);

            // Re-enable plane detection
            if (planeManager != null)
            {
                planeManager.enabled = true;
                foreach (var plane in planeManager.trackables)
                {
                    plane.gameObject.SetActive(true);
                }
            }
        }
    }

    public void DeletePlacedObject()
    {
        if (customPlacement != null)
        {
            customPlacement.DeleteSpawnedObject();
        }
    }

    public void ResetPlaneDetection()
    {
        if (planeManager != null)
        {
            planeManager.enabled = false;
            foreach (var plane in planeManager.trackables)
            {
                Destroy(plane.gameObject);
            }
            planeManager.enabled = true;
        }
    }

    public void FullReset()
    {
        DeletePlacedObject();
        if (arSession != null)
        {
            arSession.Reset();
        }
        if (planeManager != null)
        {
            planeManager.enabled = false;
            foreach (var plane in planeManager.trackables)
            {
                Destroy(plane.gameObject);
            }
            planeManager.enabled = true;
        }
    }

    public void QuickReset()
    {
        DeletePlacedObject();
        ResetPlaneDetection();
    }
}