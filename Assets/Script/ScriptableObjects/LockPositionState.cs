using UnityEngine;

[CreateAssetMenu(fileName = "LockPositionState", menuName = "AR/LockPositionState")]
public class LockPositionState : ScriptableObject
{
    [SerializeField]
    private bool isUnlocked = true; // Default to unlocked state

    private void OnEnable()
    {
        // Ensure it starts unlocked when the scriptable object is created/enabled
        isUnlocked = true;
    }

    public bool IsUnlocked
    {
        get => isUnlocked;
        set
        {
            isUnlocked = value;
            OnLockStateChanged?.Invoke(isUnlocked);
        }
    }

    // Event to notify when state changes
    public delegate void LockStateChanged(bool isUnlocked);
    public event LockStateChanged OnLockStateChanged;

    // Reset to unlocked state
    public void ResetToDefault()
    {
        isUnlocked = true;
    }
}