using UnityEngine;
using System;

[CreateAssetMenu(fileName = "ScadaDataTrainerkit", menuName = "AR/SCADA Data Storage Trainerkit")]
public class ScadaDataStorageTrainerkit : ScriptableObject
{
    // Default values set in the inspector
    public bool RedLamp = false;
    public bool YellowLamp = false;
    public bool GreenLamp = false;
    public bool BuzzerLamp = false;
    public bool Fan = false;
    public bool RedButton = false;
    public bool GreenButton = false;
    public bool Switch1 = false;
    public bool Switch2 = false;
    public int ThermoCople = 0;
    public int CurrentInjector = 0;

    public event Action OnDataUpdated;

    private void OnEnable()
    {
        // Reset all values to defaults when the game starts
        ResetToDefaults();
    }

    private void ResetToDefaults()
    {
        RedButton = false;
        YellowLamp = false;
        GreenLamp = false;
        BuzzerLamp = false;
        Fan = false;
        RedLamp = false;
        GreenButton = false;
        Switch1 = false;
        Switch2 = false;
        ThermoCople = 0;
        CurrentInjector = 0;

        // Notify any listeners that data has been reset
        NotifyDataUpdated();
    }

    public void NotifyDataUpdated()
    {
        OnDataUpdated?.Invoke();
    }

    // Optional: Public method to manually reset if needed
    public void Reset()
    {
        ResetToDefaults();
    }
}