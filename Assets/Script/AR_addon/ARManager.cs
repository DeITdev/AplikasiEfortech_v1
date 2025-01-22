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

    private bool isObjectSpawned = false;

    private void Awake()
    {
        customPlacement = FindObjectOfType<CustomARPlacement>();
        planeManager = FindObjectOfType<ARPlaneManager>();
        arSession = FindObjectOfType<ARSession>();

        // Initially hide all buttons
        if (deleteButton != null) deleteButton.SetActive(false);
        if (graphButton != null) graphButton.SetActive(false);
        if (noGraphButton != null) noGraphButton.SetActive(false);

        // Subscribe to spawn events
        CustomARPlacement.OnObjectSpawned += HandleObjectSpawned;
        CustomARPlacement.OnObjectDestroyed += HandleObjectDestroyed;

        // Initialize graph state subscription
        if (graphState != null)
        {
            graphState.OnGraphStateChanged += UpdateGraphButtonVisibility;
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
    }

    private void HandleObjectSpawned()
    {
        isObjectSpawned = true;
        if (deleteButton != null)
        {
            deleteButton.SetActive(true);
        }
        // Update graph buttons visibility based on current state
        if (graphState != null)
        {
            UpdateGraphButtonVisibility(graphState.IsGraphVisible);
        }
    }

    private void HandleObjectDestroyed()
    {
        isObjectSpawned = false;
        if (deleteButton != null)
        {
            deleteButton.SetActive(false);
        }
        // Hide both graph buttons
        if (graphButton != null) graphButton.SetActive(false);
        if (noGraphButton != null) noGraphButton.SetActive(false);
    }

    private void UpdateGraphButtonVisibility(bool isGraphVisible)
    {
        // Only show graph buttons if an object is spawned
        if (isObjectSpawned)
        {
            if (graphButton != null)
            {
                graphButton.SetActive(isGraphVisible);
            }
            if (noGraphButton != null)
            {
                noGraphButton.SetActive(!isGraphVisible);
            }
        }
        else
        {
            // Hide both buttons if no object is spawned
            if (graphButton != null) graphButton.SetActive(false);
            if (noGraphButton != null) noGraphButton.SetActive(false);
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