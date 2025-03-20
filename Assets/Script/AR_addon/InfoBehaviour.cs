using UnityEngine;

public class InfoBehaviour : MonoBehaviour
{
    const float SPEED = 6f;

    [Header("Section Info Settings")]
    [SerializeField] Transform[] sectionInfos; // Array of transforms to scale

    [Header("Graph State Reference")]
    [SerializeField] private GraphState graphState; // Reference to the scriptable object

    private Vector3[] desiredScales; // Array to store desired scale for each info section
    private bool isOpen = false;

    void Start()
    {
        // Initialize the desired scales array
        desiredScales = new Vector3[sectionInfos.Length];

        // Check if we should apply default scaling behavior (scale down to zero)
        bool shouldScaleDown = graphState == null || !graphState.IsGraphVisible;

        for (int i = 0; i < desiredScales.Length; i++)
        {
            // If graph is visible, initialize at scale 1, otherwise at scale 0
            desiredScales[i] = shouldScaleDown ? Vector3.zero : Vector3.one;

            // Apply initial scale immediately
            if (sectionInfos[i] != null)
            {
                sectionInfos[i].localScale = desiredScales[i];
            }
        }

        isOpen = !shouldScaleDown;

        // Subscribe to graph state changes
        if (graphState != null)
        {
            graphState.OnGraphStateChanged += OnGraphStateChanged;
        }
    }

    void OnDestroy()
    {
        // Unsubscribe from event to prevent memory leaks
        if (graphState != null)
        {
            graphState.OnGraphStateChanged -= OnGraphStateChanged;
        }
    }

    // Handle graph state changes
    private void OnGraphStateChanged(bool isVisible)
    {
        if (isVisible)
        {
            // When graph becomes visible, set all scales to one
            for (int i = 0; i < desiredScales.Length; i++)
            {
                desiredScales[i] = Vector3.one;
            }
            isOpen = true;
        }
    }

    void Update()
    {
        // Skip scaling only if graph state is true
        if (graphState != null && graphState.IsGraphVisible)
        {
            // Keep everything at scale 1 if graph is visible
            for (int i = 0; i < sectionInfos.Length; i++)
            {
                if (sectionInfos[i] != null && sectionInfos[i].localScale != Vector3.one)
                {
                    sectionInfos[i].localScale = Vector3.one;
                }
            }
            return;
        }

        // If we get here, apply normal gaze interaction scaling behavior
        for (int i = 0; i < sectionInfos.Length; i++)
        {
            if (sectionInfos[i] != null)
            {
                sectionInfos[i].localScale = Vector3.Lerp(
                    sectionInfos[i].localScale,
                    desiredScales[i],
                    Time.deltaTime * SPEED
                );
            }
        }
    }

    public void OpenInfo()
    {
        // Skip if graph is visible (always kept open)
        if (graphState != null && graphState.IsGraphVisible)
        {
            isOpen = true;
            return;
        }

        // Normal gaze interaction
        for (int i = 0; i < desiredScales.Length; i++)
        {
            desiredScales[i] = Vector3.one;
        }
        isOpen = true;
    }

    public void CloseInfo()
    {
        // Skip if graph is visible (always kept open)
        if (graphState != null && graphState.IsGraphVisible)
        {
            return;
        }

        // Normal gaze interaction
        for (int i = 0; i < desiredScales.Length; i++)
        {
            desiredScales[i] = Vector3.zero;
        }
        isOpen = false;
    }

    public bool IsOpen()
    {
        return isOpen;
    }
}