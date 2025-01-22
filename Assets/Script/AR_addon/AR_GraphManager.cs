using UnityEngine;
using UnityEngine.Events;

public class AR_GraphManager : MonoBehaviour
{
    [SerializeField] private GraphState graphState;

    [Header("Events")]
    [SerializeField] private UnityEvent onGraphVisible;     // Called when graph becomes visible
    [SerializeField] private UnityEvent onGraphInvisible;   // Called when graph becomes invisible

    private void OnEnable()
    {
        if (graphState != null)
        {
            graphState.OnGraphStateChanged += HandleGraphStateChanged;
            HandleGraphStateChanged(graphState.IsGraphVisible);
        }
    }

    private void OnDisable()
    {
        if (graphState != null)
        {
            graphState.OnGraphStateChanged -= HandleGraphStateChanged;
        }
    }

    private void HandleGraphStateChanged(bool isVisible)
    {
        // Invoke the appropriate event based on visibility
        if (isVisible)
        {
            onGraphVisible?.Invoke();
        }
        else
        {
            onGraphInvisible?.Invoke();
        }
    }

    public void ToggleGraphVisibility()
    {
        if (graphState != null)
        {
            graphState.IsGraphVisible = !graphState.IsGraphVisible;
        }
    }
}