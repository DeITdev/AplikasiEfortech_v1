using UnityEngine;

[CreateAssetMenu(fileName = "GraphState", menuName = "AR/GraphState")]
public class GraphState : ScriptableObject
{
    // Set default value to true so graph is visible by default
    [SerializeField]
    private bool isGraphVisible = true;

    private void OnEnable()
    {
        // Ensure it starts visible when the scriptable object is created/enabled
        isGraphVisible = true;
    }

    public bool IsGraphVisible
    {
        get => isGraphVisible;
        set
        {
            isGraphVisible = value;
            OnGraphStateChanged?.Invoke(isGraphVisible);
        }
    }

    // Event to notify when state changes
    public delegate void GraphStateChanged(bool isVisible);
    public event GraphStateChanged OnGraphStateChanged;

    // Reset to visible state
    public void ResetToDefault()
    {
        isGraphVisible = true;
    }
}